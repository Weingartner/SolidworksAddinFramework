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
        public PmpBase.StateEnum State { get; }

        public SampleMacroFeature() : this(PmpBase.StateEnum.Edit)
        {
        }
        public SampleMacroFeature(PmpBase.StateEnum state)
        {
            State = state;
        }

        public override SampleMacroFeatureDataBase Database { get; set; }

        protected override object Edit()
        {
            ppage = new SamplePropertyPage(this, State);
            if (SwFeatureData != null)
            {
                var result = SwFeatureData.AccessSelections(ModelDoc, null);
                if(!result)
                    throw new Exception("Expected to get true");
                {
                    object objects;
                    object objectTypes;
                    object marks;
                    object drViews;
                    object componentXForms;
                    SwFeatureData.GetSelections3(out objects, out objectTypes, out marks, out drViews, out componentXForms);

                    if(objects!=null)
                    {
                        var objectsArray = ((object[])objects).Cast<IBody2>().ToList();
                        swSelectType_e[] typesArray = (swSelectType_e[])objectTypes;

                        ModelDoc.ClearSelection2(true);
                        foreach (var feature in objectsArray)
                        {
                            feature.Select2(true, null);
                        }
                        
                    }
                }

            }

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
            var macroFeature = new SampleMacroFeature(PmpBase.StateEnum.Insert);
            macroFeature.Edit(app, moddoc, null);

            return true;
        
        }
    }
}