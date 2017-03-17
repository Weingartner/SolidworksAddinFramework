using System;
using System.Runtime.InteropServices;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using WeinCadSW.MacroFeatures.CurveBender;
using Vector3 = System.DoubleNumerics.Vector3;

namespace DemoMacroFeatures.MultiWireBodies
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISwComFeature))]
    public class MultiWireBodies : MacroFeatureBase<MultiWireBodies, MulitWireBodiesData>
    {
        public override string FeatureName { get; } = "MultiWireBodies";

        protected override swMacroFeatureOptions_e FeatureOptions { get; } =
            swMacroFeatureOptions_e.swMacroFeatureByDefault;


        protected override PropertyManagerPageBase GetPropertyManagerPage()
        {
            return new MultiWireBodiesPmp(this);
        }

        protected override object Security()
        {
            return null;
        }

        public MultiWireBodies()
        {
            Console.WriteLine("Constructed");
        }

        private Random _R = new Random();

        protected override object Regenerate(IModeler modeler)
        {
            SwFeatureData.EnableMultiBodyConsume = true;

            if (Database.Style == MulitWireBodiesData.StyleEnum.Wire)
            {
                var w = _R.NextDouble();

                var line0 = modeler.CreateTrimmedLine(new Vector3(0, -w, 0), new Vector3(1, -w, 0));
                var line1 = modeler.CreateTrimmedLine(new Vector3(0, w, 0), new Vector3(1, w, 0));

                var wire0 = line0.CreateWireBody();
                var wire1 = line1.CreateWireBody();

                SwFeatureData.AddIdsToBody(wire0);
                SwFeatureData.AddIdsToBody(wire1);

                return new[] { wire0, wire1 };

            }
            else
            {
                var solid0 = modeler.CreateBox(new Vector3(0, 0, 0), Vector3.UnitX, 1, 1, 1);
                var solid1 = modeler.CreateBox(new Vector3(2, 0, 0), Vector3.UnitX, 1, 1, 1);

                SwFeatureData.AddIdsToBody(solid0);
                SwFeatureData.AddIdsToBody(solid0);

                return new[] { solid0, solid1 };
            }

        }

        public static
            void AddMacroFeature(ISldWorks app)
        {
            var moddoc = (IModelDoc2) app.ActiveDoc;
            var macroFeature = new MultiWireBodies();
            macroFeature.Edit(app, moddoc, null);
        }
    }
}
