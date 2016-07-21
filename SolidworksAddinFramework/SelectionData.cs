using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using LanguageExt;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static LanguageExt.Prelude;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Weingartner.Json.Migration;

namespace SolidworksAddinFramework
{
    [DataContract]
    [Migratable("892617508")]
    public class SelectionData
    {
        private static bool Equals(IEnumerable<ObjectId> o1, IEnumerable<ObjectId> o2) => o1.SequenceEqual(o2);

        private bool Equals(SelectionData other)
        {
            return Equals(ObjectIds, other.ObjectIds) && Mark == other.Mark;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((SelectionData) obj);
        }

        private static int GetHashCode(IEnumerable<ObjectId> a) => a.GetHashCode(v => v.GetHashCode());

        public override int GetHashCode() => ObjectExtensions.GetHashCode(GetHashCode(ObjectIds), Mark);

        public static readonly SelectionData Empty = new SelectionData(Enumerable.Empty<ObjectId>(), -1);

        public override string ToString() => $"{ObjectIds.Count} selections, Mark {Mark}";

        [DataMember]
        public IReadOnlyList<ObjectId> ObjectIds { get; }

        [DataMember]
        public int Mark { get; }

        public bool IsEmpty => ObjectIds.Count == 0;

        public SelectionData(IEnumerable<ObjectId> objectIds, int mark)
        {
            ObjectIds = new ReadOnlyCollection<ObjectId>(objectIds.ToList());
            Mark = mark;
        }

        public static SelectionData Create(IEnumerable<object> objects, int mark, IModelDoc2 doc)
        {
            var objectIds = objects
                .Select(doc.GetPersistReference)
                .Select(id => new ObjectId(id));
            return new SelectionData(objectIds, mark);
        }

        [DataContract]
        public class ObjectId
        {
            private readonly byte[] _Data;

            [DataMember]
            public byte[] Data => _Data.ToArray();

            public ObjectId([NotNull] byte[] data)
            {
                if (data == null) throw new ArgumentNullException(nameof(data));

                _Data = data.ToArray();
            }


            public override string ToString()
            {
                return BitConverter.ToString(_Data);
            }

            private static bool Equals(IEnumerable<byte> o1, IEnumerable<byte> o2) => o1.SequenceEqual(o2);

            private bool Equals(ObjectId other)
            {
                return Equals(Data, other.Data);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((ObjectId) obj);
            }

            public override int GetHashCode()
            {
                return Data.GetHashCode(p => p);
            }
        }

        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        private static JObject Migrate_1(JObject data, JsonSerializer serializer)
        {
            data["ObjectIds"] = new JArray(data["ObjectIds"].Select(d => new JObject { { "Data", d } }));
            return data;
        }
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local
    }

    public static class SelectionDataExtensions
    {
        public static IEnumerable<object> GetObjects(this SelectionData selectionData, IModelDoc2 doc)
        {
            return selectionData.ObjectIds
                .Select(objectId => doc.GetObjectFromPersistReference(objectId.Data));
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

        public static IEnumerable<SelectionData> GetSelectionsFromModel(object model)
        {
            return model
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof (SelectionData))
                .Select(p => p.GetValue(model))
                .Cast<SelectionData>();
        }

        public static SelectionData WithObjectIds(this SelectionData selectionData, IEnumerable<SelectionData.ObjectId> objectIds)
        {
            return new SelectionData(objectIds, selectionData.Mark);
        }
    }
}