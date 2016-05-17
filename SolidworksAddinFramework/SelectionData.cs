using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using LanguageExt;
using Newtonsoft.Json;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    [DataContract]
    public class SelectionData
    {
        public static readonly SelectionData Empty = new SelectionData(Enumerable.Empty<byte[]>(), -1);

        [DataMember]
        public IReadOnlyList<byte[]> ObjectIds { get; }
        [DataMember]
        public int Mark { get; }

        public bool IsEmpty => ObjectIds.Count == 0;

        public SelectionData(IEnumerable<byte[]> objectIds, int mark)
        {
            ObjectIds = new ReadOnlyCollection<byte[]>(objectIds.ToList());
            Mark = mark;
        }
    }

    public static class SelectionDataExtensions
    {
        public static IEnumerable<object> GetObjects(this SelectionData selectionData, IModelDoc2 doc)
        {
            return selectionData.ObjectIds
                .Select(objectId =>
                {
                    int errorCode;
                    var @object = doc.Extension.GetObjectByPersistReference3(objectId, out errorCode);
                    var result = (swPersistReferencedObjectStates_e) errorCode;
                    if (result != swPersistReferencedObjectStates_e.swPersistReferencedObject_Ok)
                    {
                        throw new SelectionException($"GetObjectByPersistReference3 returned {result}");
                    }
                    return @object;
                });
        }

        public static Option<object> GetSingleObject(this SelectionData selectionData, IModelDoc2 doc)
        {
            return selectionData.GetObjects(doc).FirstOrDefault();
        }
        public static Option<T> GetSingleObject<T>(this SelectionData selectionData, IModelDoc2 doc)
        {
            return GetSingleObject(selectionData, doc).DirectCast<T>();
        }

        public static SelectionData SetObjects(this SelectionData selectionData, IEnumerable<object> objects, IModelDoc2 doc)
        {
            var objectIds = objects.Select(o => doc.Extension.GetPersistReference3(o).CastArray<byte>());
            return new SelectionData(objectIds, selectionData.Mark);
        }

        public static IEnumerable<SelectionData> GetSelectionsFromModel(object model)
        {
            return model
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof (SelectionData))
                .Select(p => p.GetValue(model))
                .Cast<SelectionData>();
        }
    }
}