using System.Runtime.Serialization;

namespace SolidworksAddinFramework
{
    [DataContract]
    public class SelectionData
    {
        [DataMember]
        public byte[] ObjectId { get; private set; }
        [DataMember]
        public int Mark { get; private set; }

        public SelectionData(byte[] objectId, int mark)
        {
            ObjectId = objectId;
            Mark = mark;
        }
    }
}