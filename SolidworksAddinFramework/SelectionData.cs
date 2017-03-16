using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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