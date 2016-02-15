using System;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SwCSharpAddinMF
{
    public class MacroFeature : ISwComFeature
    {
        private IModelDoc2 iModelDoc = null;
        private UserPmPage ppage = null;

        public MacroFeature()
        {
        }

        Object ISwComFeature.Edit(Object app, Object modelDoc, Object feature)
        {
            SwApp = app as ISldWorks;
            iModelDoc = modelDoc as IModelDoc2;
            
            ppage = new UserPmPage(SwApp);
            ppage.Show();
            return null;
        }

        public ISldWorks SwApp { get; set; }

        Object ISwComFeature.Regenerate(Object app, Object modelDoc, Object feature)
        {
            SwApp = app as ISldWorks;
            iModelDoc = modelDoc as IModelDoc2;

            MessageBox.Show("MF Regenerate");
            return null;
        }

        Object ISwComFeature.Security(Object app, Object modelDoc, Object feature)
        {
            SwApp = app as ISldWorks;
            iModelDoc = modelDoc as IModelDoc2;

            MessageBox.Show("MF Security");
            return null;
        }

        public Boolean AddMacroFeature(ISldWorks app) 
        {
            object paramNames;
            object paramTypes;
            object paramValues;
            var TparamNames = new string[3];
            var TparamTypes = new int[3]; //Use int for 64 bit compatibility
            var TparamValues = new string[3];

            var moddoc = (IModelDoc2) app.ActiveDoc;
            var FeatMgr = (IFeatureManager)moddoc.FeatureManager;

            //Include only data that won't be available from geometry
            TparamNames[0] = "Width";
            TparamNames[1] = "Offset";
            TparamNames[2] = "Depth";

            TparamTypes[0] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;
            TparamTypes[1] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;
            TparamTypes[2] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;

            //Hard code the parameters for test,
            //but in practice get this from Property Manager Page
            TparamValues[0] = "0.01"; //Width
            TparamValues[1] = "0.005"; //Offset
            TparamValues[2] = "0.006"; //Depth

            paramNames = TparamNames;
            paramTypes = TparamTypes;
            paramValues = TparamValues;

            IBody2 editBody = null;

            var opts = 0;

            IFeature MacroFeature = FeatMgr.InsertMacroFeature3("ORingGroove", nameof(SwCSharpAddinMF) + "." + nameof(MacroFeature), null, (paramNames), (paramTypes), (paramValues), null, null, editBody, null, opts);

            TparamNames = null;
            TparamTypes = null;
            TparamValues = null;

            return true;
        
        }
    }
}