using System.Runtime.Serialization;
using Newtonsoft.Json;
using ReactiveUI;
using SolidworksAddinFramework;
using Weingartner.WeinCad.Interfaces;

namespace DemoMacroFeatures.SampleMacroFeature
{
    [DataContract]
    public class SampleMacroFeatureDataBase : ReactiveObject
    {
        private double _Alpha;


        [DataMember]
        public double Alpha 
        {
            get { return _Alpha; }
            set { this.RaiseAndSetIfChanged(ref _Alpha, value); }
        }

        private SelectionData _Body = SelectionData.Empty;

        [DataMember]
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public SelectionData Body 
        {
            get { return _Body; }
            set { this.RaiseAndSetIfChanged(ref _Body, value); }
        }
    }
}