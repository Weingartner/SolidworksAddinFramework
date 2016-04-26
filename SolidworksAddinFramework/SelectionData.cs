using System.Runtime.Serialization;
using ReactiveUI;

namespace SolidworksAddinFramework
{
    [DataContract]
    public class SelectionData : ReactiveObject
    {
        [DataMember]
        public string ObjectName { get; private set; }
        [DataMember]
        public string TypeName { get; private set; }
        [DataMember]
        public double X { get; private set; }
        [DataMember]
        public double Y { get; private set; }
        [DataMember]
        public double Z { get; private set; }
        [DataMember]
        public int Mark { get; private set; }

        public SelectionData(string objectName, string typeName, double x, double y, double z, int mark)
        {
            ObjectName = objectName;
            TypeName = typeName;
            X = x;
            Y = y;
            Z = z;
            Mark = mark;
        }
    }
}