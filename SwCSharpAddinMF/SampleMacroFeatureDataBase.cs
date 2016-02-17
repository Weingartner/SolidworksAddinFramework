using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;

namespace SwCSharpAddinMF
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

        [MacroFeatureDataField]
        public string Param0 { get; set; } = "hello";

        [MacroFeatureDataField]
        public double Param1 { get; set; } = 1.1;

        [MacroFeatureDataField]
        public bool Param2 { get; set; } = true;

        [MacroFeatureDataField]
        public int Param3 { get; set; } = 1;

        [MacroFeatureDataField]
        public int ListItem { get; set; }

        [MacroFeatureDataField]
        public int ComboBoxItem { get; set; }

        // This doesn't need to be serialized
        public object[] SelectedObjects { get; set; } = new object[] {};
    }
}