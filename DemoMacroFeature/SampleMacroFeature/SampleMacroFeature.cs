using System.Numerics;
using System.Runtime.InteropServices;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace DemoMacroFeatures.SampleMacroFeature
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISwComFeature))]
    public class SampleMacroFeature : MacroFeatureBase<SampleMacroFeature,SampleMacroFeatureDataBase>
    {
        private SampleMacroFeature()
            : base("Alpha Split", swMacroFeatureOptions_e.swMacroFeatureByDefault)
        {
        }

        protected override PropertyManagerPageBase GetPropertyManagerPage() => new SamplePropertyPage(this);

        protected override object Regenerate(IModeler modeler)
        {
            var body = (IBody2)Database.Body.GetSingleObject(ModelDoc);
            if (body == null)
                return null;

            body = (IBody2)body.Copy();
            SwFeatureData.EnableMultiBodyConsume = true;
            var splitBodies = SplitBodies(modeler, body, Database);
            if (splitBodies == null) return "There was some error";

            foreach (var splitBody in splitBodies)
            {
                SwFeatureData.AddIdsToBody(splitBody);
            }

            return splitBodies;
        }

        public static IBody2[] SplitBodies(IModeler modeler, IBody2 body, SampleMacroFeatureDataBase database)
        {
            var box = body.GetBodyBoxTs();
            var center = box.Center;
            var axisX = Vector3.UnitX;

            // Find the point to cut the object
            center.X = (float) (database.Alpha*box.P0.X + (1 - database.Alpha)*box.P1.X);
            var sheet = modeler.CreateSheet(center, axisX, box.P0, box.P1);

            var cutResult = body.Cut(sheet);
            return cutResult.Error == 0 ? cutResult.Bodies : null;
        }

        protected override object Security()
        {
            return null;
        }

        public static bool AddMacroFeature(ISldWorks app) 

        {
            var moddoc = (IModelDoc2) app.ActiveDoc;
            var macroFeature = new SampleMacroFeature();
            macroFeature.Edit(app, moddoc, null);
            return true;
        }
    }
}