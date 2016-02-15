using System;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SwCSharpAddinMF
{
    public class SampleMacroFeature : ISwComFeature
    {
        private SamplePropertyPage ppage = null;

        public SampleMacroFeature()
        {
        }

        object ISwComFeature.Edit(object app, object modelDoc, object feature)
        {
            SwApp = app as ISldWorks;

            ppage = new SamplePropertyPage(SwApp);
            ppage.Show();
            return null;
        }

        public ISldWorks SwApp { get; set; }

        object ISwComFeature.Regenerate(object app, object modelDoc, object feature)
        {
            SwApp = app as ISldWorks;

            MessageBox.Show("MF Regenerate");
            return null;
        }

        object ISwComFeature.Security(object app, object modelDoc, object feature)
        {
            SwApp = app as ISldWorks;

            MessageBox.Show("MF Security");
            return null;
        }

        public bool AddMacroFeature(ISldWorks app) 
        {
            var names = new string[3];
            var types = new int[3]; //Use int for 64 bit compatibility
            var values = new string[3];

            var moddoc = (IModelDoc2) app.ActiveDoc;
            var featMgr = (IFeatureManager)moddoc.FeatureManager;

            //Include only data that won't be available from geometry
            names[0] = "Width";
            names[1] = "Offset";
            names[2] = "Depth";

            types[0] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;
            types[1] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;
            types[2] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;

            //Hard code the parameters for test,
            //but in practice get this from Property Manager Page
            values[0] = "0.01"; //Width
            values[1] = "0.005"; //Offset
            values[2] = "0.006"; //Depth

            object paramNames = names;
            object paramTypes = types;
            object paramValues = values;

            IBody2 editBody = null;

            const int opts = 0;

            IFeature MacroFeature = featMgr.InsertMacroFeature3("ORingGroove", nameof(SwCSharpAddinMF) + "." + nameof(MacroFeature), null, (paramNames), (paramTypes), (paramValues), null, null, editBody, null, opts);

            names = null;
            types = null;
            values = null;

            return true;
        
        }
    }
}