using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
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
        public override string FeatureName { get; } = "Sample Feature";
        public override swMacroFeatureOptions_e FeatureOptions { get; } = 0;
        public override IBody2 EditBody { get; } = null;

        protected override object Edit()
        {
            ppage = new SamplePropertyPage(this);
            ppage.Show();
            return null;
        }

        protected override object Regenerate()
        {
            //MessageBox.Show("MF Regenerate");
            return null;
        }

        protected override object Security()
        {
            //MessageBox.Show("MF Security");
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