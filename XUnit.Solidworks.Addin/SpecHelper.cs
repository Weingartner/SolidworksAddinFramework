using FluentAssertions;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace XUnit.Solidworks.Addin
{
    public static class SpecHelper
    {
        public static Feature InsertDummyBody(IModelDoc2 modelDoc)
        {
            modelDoc.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, true, 0, null, 0).Should().BeTrue();
            var plane1 = (RefPlane)modelDoc.FeatureManager.InsertRefPlane(8, 0.01, 0, 0, 0, 0);
            modelDoc.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, true, 0, null, 0).Should().BeTrue();
            var plane2 = (RefPlane)modelDoc.FeatureManager.InsertRefPlane(8, 0.02, 0, 0, 0, 0);

            modelDoc.Extension.SelectByID2("Plane2", "PLANE", 0, 0, 0, false, 0, null, 0).Should().BeTrue();
            var lines =
                modelDoc.SketchManager.CreateCornerRectangle(-0.02, 0.01, 0, 0.02, -0.01, 0)
                    .CastArray<ISketchSegment>();
            // Sketch to extrude
            modelDoc.Extension.SelectByID2("Sketch1", "SKETCH", 0, 0, 0, false, 0, null, 0).Should().BeTrue();
            // Start condition reference
            modelDoc.Extension.SelectByID2("Plane2", "PLANE", 0.00105020593408751, -0.00195369982668282,
                0.0248175428318827, true, 32, null, 0).Should().BeTrue();
            // End condition reference
            modelDoc.Extension.SelectByID2("Plane1", "PLANE", 0.0068370744701368, -0.004419862088339,
                0.018892268568016, true, 1, null, 0).Should().BeTrue();

            // Boss extrusion start condition reference is Plane2, and the extrusion end is offset 3 mm from the end condition reference, Plane1
            return modelDoc.FeatureManager.FeatureExtrusion3(true, false, true,
                (int)swEndConditions_e.swEndCondOffsetFromSurface, 0, 0.003, 0.003, false, false, false,
                false, 0.0174532925199433, 0.0174532925199433, false, false, false, false, true, true, true,
                (int)swStartConditions_e.swStartSurface, 0, false);
        }
    }
}
