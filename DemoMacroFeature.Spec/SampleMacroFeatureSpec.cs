using System;
using System.Linq;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Xunit;
using XUnit.Solidworks.Addin;

namespace SwCSharpAddinMF.Spec
{
    public class SampleMacroFeatureSpec : SolidWorksSpec
    {

        [SolidworksFact]
        public void ShouldBeAbleToApplySampleMacroFeature()
        {
            ((IModelDoc2) SwApp.NewPart()).Using(SwApp,modelDoc =>
            {

                Assert.True(modelDoc.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, true, 0, null, 0));
                var plane1 = (RefPlane) modelDoc.FeatureManager.InsertRefPlane(8, 0.01, 0, 0, 0, 0);
                Assert.True(modelDoc.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, true, 0, null, 0));
                var plane2 = (RefPlane) modelDoc.FeatureManager.InsertRefPlane(8, 0.02, 0, 0, 0, 0);

                Assert.True(modelDoc.Extension.SelectByID2("Plane2", "PLANE", 0, 0, 0, false, 0, null, 0));
                var lines =
                    modelDoc.SketchManager.CreateCornerRectangle(-0.02, 0.01, 0, 0.02, -0.01, 0)
                        .CastArray<ISketchSegment>();
                // Sketch to extrude
                Assert.True(modelDoc.Extension.SelectByID2("Sketch1", "SKETCH", 0, 0, 0, false, 0, null, 0));
                // Start condition reference
                Assert.True(modelDoc.Extension.SelectByID2("Plane2", "PLANE", 0.00105020593408751, -0.00195369982668282,
                    0.0248175428318827, true, 32, null, 0));
                // End condition reference
                Assert.True(modelDoc.Extension.SelectByID2("Plane1", "PLANE", 0.0068370744701368, -0.004419862088339,
                    0.018892268568016, true, 1, null, 0));

                // Boss extrusion start condition reference is Plane2, and the extrusion end is offset 3 mm from the end condition reference, Plane1
                var feature = modelDoc.FeatureManager.FeatureExtrusion3(true, false, true,
                    (int) swEndConditions_e.swEndCondOffsetFromSurface, 0, 0.003, 0.003, false, false, false,
                    false, 0.0174532925199433, 0.0174532925199433, false, false, false, false, true, true, true,
                    (int) swStartConditions_e.swStartSurface, 0, false);

                var macroFeature = new DemoMacroFeatures.SampleMacroFeature.SampleMacroFeature();
                macroFeature.Edit(SwApp, modelDoc, null);
                Assert.True(modelDoc.Extension.SelectByID2(feature.Name, "SOLIDBODY", 0, 0, 0, false, 1, null, 0));

                // TODO this doesn't work. It raises a dialog that says it can't create
                // the feature
                macroFeature.Commit();
            });
        }
    }
}
