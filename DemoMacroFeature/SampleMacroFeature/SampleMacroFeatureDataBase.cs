using ReactiveUI;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;

namespace DemoMacroFeatures.SampleMacroFeature
{
    public class SampleMacroFeatureDataBase : MacroFeatureDataBase
    {
        public SampleMacroFeatureDataBase(IMacroFeatureData featureData)
        {
            ReadFrom(featureData);
        }
        public SampleMacroFeatureDataBase()
        {
        }

        string _Param0="";
        [MacroFeatureDataField]
        public string Param0 
        {
            get { return _Param0; }
            set { this.RaiseAndSetIfChanged(ref _Param0, value); }
        }

        float _Alpha;
        [MacroFeatureDataField]
        public float Alpha 
        {
            get { return _Alpha; }
            set { this.RaiseAndSetIfChanged(ref _Alpha, value); }
        }

        bool _Param2;
        [MacroFeatureDataField]
        public bool Param2 
        {
            get { return _Param2; }
            set { this.RaiseAndSetIfChanged(ref _Param2, value); }
        }

        int _Param3;
        [MacroFeatureDataField]
        public int Param3 
        {
            get { return _Param3; }
            set { this.RaiseAndSetIfChanged(ref _Param3, value); }
        }

        int _ListItem;
        [MacroFeatureDataField]
        public int ListItem 
        {
            get { return _ListItem; }
            set { this.RaiseAndSetIfChanged(ref _ListItem, value); }
        }

        int _ComboBoxyItem;
        [MacroFeatureDataField]
        public int ComboBoxyItem 
        {
            get { return _ComboBoxyItem; }
            set { this.RaiseAndSetIfChanged(ref _ComboBoxyItem, value); }
        }

    }
}