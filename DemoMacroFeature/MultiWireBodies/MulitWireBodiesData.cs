using System.Runtime.Serialization;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SolidworksAddinFramework;

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
