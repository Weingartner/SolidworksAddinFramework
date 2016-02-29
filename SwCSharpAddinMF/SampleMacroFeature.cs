using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SwCSharpAddinMF
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISwComFeature))]
    public class SampleMacroFeature : MacroFeatureBase<SampleMacroFeature,SampleMacroFeatureDataBase>
    {
        private SamplePropertyPage _Ppage = null;

        public SampleMacroFeature() 
        {
        }

        public override SampleMacroFeatureDataBase Database { get; set; }
        public override string FeatureName { get; } = "Sample Feature";
        public override swMacroFeatureOptions_e FeatureOptions { get; } = 0;
        public override List<IBody2> EditBodies => SelectedBodies();

        protected override bool Edit()
        {
            _Ppage = new SamplePropertyPage(this);
            _Ppage.Show();
            return true;
        }
        private List<IBody2> SelectedBodies()
        {
            return SelectionMgr.GetSelectedObjects((type, mark) => type == swSelectType_e.swSelSOLIDBODIES)
                .Select(v => (IBody2) v)
                .ToList();
        }

        protected override object Regenerate(IModeler modeler)
        {
            // Get the body to edit
            var body = (IBody2) SwFeatureData.EditBody.Copy();
            var box = body.GetBodyBoxTs();
            var center = box.Center;
            var axisX = new double[] {1, 0, 0};

            // Find the point to cut the object
            center[0] = Database.Alpha*box.P0[0] + (1 - Database.Alpha)*box.P1[0];
            var sheet = modeler.CreateSheet(center, axisX, box.P0, box.P1);

            SwFeatureData.EnableMultiBodyConsume = true;
            var cutResult = body.Cut(sheet);
            if(cutResult.Error==0)
                foreach (var body1 in cutResult.Bodies)
                {
                    SwFeatureData.AddIdsToBody(body1);
                }
            return cutResult.Bodies;
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