using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WeinCadSW.MacroFeatures.CurveBender
{
    [DataContract]
    public class MulitWireBodiesData : ReactiveObject
    {
        public enum StyleEnum
        {
            Solid,
            Wire
        };

        [DataMember]
        [Reactive] public StyleEnum Style { get; set; }
    }
}
