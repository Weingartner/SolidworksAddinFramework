using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using LanguageExt;
using static LanguageExt.Prelude;
using Newtonsoft.Json;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    [DataContract]
    public class SelectionData
    {
        protected bool Equals(SelectionData other)
        {
            return Equals(ObjectIds, other.ObjectIds) && Mark == other.Mark;
        }

        private static bool Equals(IReadOnlyList<byte[]> itemsA, IReadOnlyList<byte[]> itemsB)
        {
            if (itemsA.Count != itemsB.Count)
                return false;
            return itemsA
                .Zip(itemsB, (a,b)=> a.SequenceEqual(b))
                .All(x=>x);
        }

        private static int GetHashCode(IEnumerable<byte[]> a) => ObjectExtensions.GetHashCode(a, v => ObjectExtensions.GetHashCode(v, v2 => v2.GetHashCode()));

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((SelectionData) obj);
        }

        public override int GetHashCode() => ObjectExtensions.GetHashCode(GetHashCode(ObjectIds), Mark);

        public static readonly SelectionData Empty = new SelectionData(Enumerable.Empty<byte[]>(), -1);

        public override string ToString() => $"SelectionData ( {Mark} - {ObjectIdsHuman}";

        [DataMember]
        public IReadOnlyList<byte[]> ObjectIds { get; }

        public string ObjectIdsHuman => string.Join(":",ObjectIds.Select(id => BitConverter.ToString(id, 0, id.Length)));
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

        /// <summary>
        /// Gets an evaluator for the selected object. We return Func because if you return the solidworks
        /// object itself and store it you get burned by solidworks rebuilds when the object is invalidated.
        /// Only evaluate the function when you actually need the solidworks object. If the return value
        /// is None then it means that there is nothing selected.
        /// </summary>
        /// <param name="selectionData"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Option<Func<object>> GetSingleObject(this SelectionData selectionData, IModelDoc2 doc) => 
            selectionData.IsEmpty
            ? None
            : Some(fun(()=> selectionData.GetObjects(doc).First()));

        public static IEnumerable<T> GetObjects<T>(this SelectionData selectionData, IModelDoc2 doc)
        {
            return from o in selectionData.GetObjects(doc)
                select o.DirectCast<T>();
        }

        public static Option<Func<T>> GetSingleObject<T>(this SelectionData selectionData, IModelDoc2 doc)
        {
            return from o in GetSingleObject(selectionData, doc)
                select fun(()=> o().DirectCast<T>());
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