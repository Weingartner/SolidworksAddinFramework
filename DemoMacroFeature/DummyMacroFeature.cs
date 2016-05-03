using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace DemoMacroFeatures
{
    public class DummyMacroFeature : MacroFeatureBase<DummyMacroFeature, DummyMacroFeatureData>
    {
        public override string FeatureName { get; } = "DummyMacroFeature";
        protected override swMacroFeatureOptions_e FeatureOptions { get; } = swMacroFeatureOptions_e.swMacroFeatureByDefault;
        protected override PropertyManagerPageBase GetPropertyManagerPage()
        {
            return null;
        }

        protected override object Security()
        {
            return null;
        }

        protected override object Regenerate(IModeler modeler)
        {
            return null;
        }

    }
}