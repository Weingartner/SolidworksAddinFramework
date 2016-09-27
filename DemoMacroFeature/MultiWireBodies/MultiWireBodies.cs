using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using SolidworksAddinFramework;
using SolidworksAddinFramework.Geometry;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using WeinCadSW.MacroFeatures.CurveBender;

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

        protected override object Regenerate(IModeler modeler)
        {
            SwFeatureData.EnableMultiBodyConsume = true;

            var line0 = modeler.CreateTrimmedLine(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            var line1 = modeler.CreateTrimmedLine(new Vector3(0, 1, 0), new Vector3(1, 1, 0));

            var wire0 = line0.CreateWireBody();
            var wire1 = line1.CreateWireBody();

            SwFeatureData.AddIdsToBody(wire0);
            SwFeatureData.AddIdsToBody(wire1);

            return new[] {wire0, wire1};
        }


        public static void AddMacroFeature(ISldWorks app)
        {
            var moddoc = (IModelDoc2) app.ActiveDoc;
            var macroFeature = new MultiWireBodies();
            macroFeature.Edit(app, moddoc, null);
        }
    }
}
