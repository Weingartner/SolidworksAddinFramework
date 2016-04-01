using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SwCSharpAddinMF.SampleMacroFeature
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISwComFeature))]
    public class SampleMacroFeature : MacroFeatureBase<SampleMacroFeature,SampleMacroFeatureDataBase>
    {
        public override SampleMacroFeatureDataBase Database { get; set; }
        public override string FeatureName { get; } = "Sample Feature";
        public override swMacroFeatureOptions_e FeatureOptions { get; } = 0;
        public override List<IBody2> EditBodies => SelectedBodies();

        protected override PropertyManagerPageBase GetPropertyManagerPage() => new SamplePropertyPage(this);

        private List<IBody2> SelectedBodies()
        {
            return SelectionMgr.GetSelectedObjects((type, mark) => type == swSelectType_e.swSelSOLIDBODIES)
                .Select(v => (IBody2) v)
                .ToList();
        }

        protected override object Regenerate(IModeler modeler)
        {
            if (SwFeatureData.EditBody == null)
                return null;
            // Get the body to edit
            var body = (IBody2) SwFeatureData.EditBody.Copy();
            SwFeatureData.EnableMultiBodyConsume = true;
            var splitBodies = SplitBodies(modeler, body, Database);
            if(splitBodies!=null)
            foreach (var body1 in splitBodies)
            {
                SwFeatureData.AddIdsToBody(body1);
            }
            return (object)splitBodies ?? "There was some error";
        }

        public static IBody2[] SplitBodies(IModeler modeler, IBody2 body, SampleMacroFeatureDataBase database)
        {
            var box = body.GetBodyBoxTs();
            var center = box.Center;
            var axisX = new double[] {1, 0, 0};

            // Find the point to cut the object
            center[0] = database.Alpha*box.P0[0] + (1 - database.Alpha)*box.P1[0];
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