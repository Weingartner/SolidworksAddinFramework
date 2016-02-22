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
        public override List<IBody2> EditBodies => new List<IBody2>(Enumerable.Empty<IBody2>());

        protected override object Edit()
        {
            _Ppage = new SamplePropertyPage(this);
            _Ppage.Show();
            return null;
        }

        protected override object Regenerate(IModeler modeler)
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