using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SwCSharpAddinMF.SWAddin;

namespace SwCSharpAddinMF
{
    public class SampleMacroFeature : MacroFeatureBase<SampleMacroFeatureDataBase>
    {
        private SamplePropertyPage ppage = null;

        public SampleMacroFeature()
        {
        }

        public override SampleMacroFeatureDataBase Database { get; set; }

        protected override object Edit()
        {
            ppage = new SamplePropertyPage(this);
            ppage.Show();
            return null;
        }
        protected override object Regenerate()
        {
            MessageBox.Show("MF Regenerate");
            return null;
        }

        protected override object Security()
        {
            MessageBox.Show("MF Security");
            return null;
        }

        public static bool AddMacroFeature(ISldWorks app) 

        {

            var moddoc = (IModelDoc2) app.ActiveDoc;
            var featMgr = (IFeatureManager)moddoc.FeatureManager;

            IBody2 editBody = null;

            const int opts = 0;

            var featureName = "ORingGroove";

            FeatureManagerExtensions.InsertMacroFeature<SampleMacroFeatureDataBase>(featMgr, featureName, editBody, opts);

            return true;
        
        }
    }
}