using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SolidworksAddinFramework
{
    public enum StateEnum {  Insert, Edit }

    /// <summary>
    /// Base class for macro features. Just inherit and fill out the abstract methods. See the
    /// sample for a full working sample.
    /// </summary>
    /// <typeparam name="TMacroFeature"></typeparam>
    /// <typeparam name="TData"></typeparam>
    [ComVisible(false)]
    public abstract class MacroFeatureBase<TMacroFeature,TData> : ISwComFeature
        where TData : MacroFeatureDataBase, new()
        where TMacroFeature : MacroFeatureBase<TMacroFeature, TData>
    {
        // Store PMP in a field so the GC can't collect it
        private PropertyManagerPageBase _EditPage;

        public IModelDoc2 ModelDoc { get; set; }

        public IFeature SwFeature { get; set; }

        public IMacroFeatureData SwFeatureData { get; set; }

        public ISldWorks SwApp { get; set; }
        public abstract TData Database { get; set; }

        public abstract string FeatureName { get; }

        public abstract swMacroFeatureOptions_e FeatureOptions { get; }

        public abstract List<IBody2> EditBodies { get; }

        public ISelectionMgr SelectionMgr { get; set; }

        /// <summary>
        /// Allows an implementation to specify what property manager page to show on edit.
        /// </summary>
        /// <returns></returns>
        protected abstract PropertyManagerPageBase GetPropertyManagerPage();

        public object Edit(object app, object modelDoc, object feature)
        {
            Init(app, modelDoc, feature);
            LoadSelections();
            _EditPage = GetPropertyManagerPage();
            _EditPage.Show();
            return true;
        }

        /// <summary>
        /// This should be called on all callbacks defined in ISwComFeature to
        /// initialize all the instance variables.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="modelDoc"></param>
        /// <param name="feature"></param>
        private void Init(object app, object modelDoc, object feature)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (modelDoc == null) throw new ArgumentNullException(nameof(modelDoc));
            // feature can be null

            SwApp = (ISldWorks) app;
            Database = new TData();
            if (feature != null)
            {
                SwFeature = (IFeature) feature;
                SwFeatureData = (IMacroFeatureData) SwFeature.GetDefinition();
                Database.ReadFrom(SwFeatureData);
            }
            ModelDoc = (IModelDoc2) modelDoc;
            SelectionMgr = (ISelectionMgr) ModelDoc.SelectionManager;
        }

        public StateEnum State => SwFeatureData == null ? StateEnum.Insert : StateEnum.Edit;


        /// <summary>
        /// Save all the selections and the parameters to the macro feature
        /// </summary>
        private void Write()
        {
            SaveSelections(this);
            Database.WriteTo(SwFeatureData);
        }

        /// <summary>
        /// Save all selection and parameters and modify the definition of the macro feature
        /// </summary>
        private void ModifyDefinition()
        {
            Write();
            SwFeatureData.EditBodies = EditBodies?.ToArray() ;
            SwFeature.ModifyDefinition(SwFeatureData, ModelDoc, null);
        }

        private void ReleaseSelectionAccess()
        {
            SwFeatureData?.ReleaseSelectionAccess();
        }


        /// <summary>
        /// Insert the new macro feature into the design tree.
        /// </summary>
        /// <param name="featureName"></param>
        /// <param name="editBodies"></param>
        /// <param name="opts"></param>
        private void InsertDefinition(string featureName, IEnumerable<IBody2> editBodies, int opts)
        {

            FeatureManagerExtensions
                .InsertMacroFeature<TMacroFeature, TData>
                (ModelDoc.FeatureManager, featureName, editBodies, opts, Database);
        }


        /// <summary>
        /// Implement to perform the security function. See sample project
        /// </summary>
        /// <returns></returns>
        protected abstract object Security();
        /// <summary>
        /// Implement to perform the rebuild function. See sample project
        /// </summary>
        /// <returns></returns>
        protected abstract object Regenerate(IModeler modeler);

        #region ISWComFeature callbacks
        public object Regenerate(object app, object modelDoc, object feature)
        {
            Init(app, modelDoc, feature);
            var modeller = (IModeler) SwApp.GetModeler();
            try
            {
                return Regenerate(modeller);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public object Security(object app, object modelDoc, object feature)
        {
            Init(app, modelDoc, feature);
            return Security();
        }
        #endregion

        /// <summary>
        /// Serialize all selected objects and marks to the the macro feature data
        /// </summary>
        /// <param name="sampleMacroFeature"></param>
        private static void SaveSelections(MacroFeatureBase<TMacroFeature,TData> sampleMacroFeature)
        {
            var objects = SelectionManagerExtensions.GetSelectedObjects(sampleMacroFeature.SelectionMgr, (type, mark) => true)
                .ToArray();

            var marks =
                Enumerable.Range(1, objects.Length)
                    .Select(i => sampleMacroFeature.SelectionMgr.GetSelectedObjectMark(i))
                    .ToArray();

            sampleMacroFeature.SwFeatureData.SetSelections(ComWangling.ObjectArrayToDispatchWrapper(objects), marks);
            Debug.Assert(sampleMacroFeature.SwFeatureData.GetSelectionCount() == objects.Length);
        }

        /// <summary>
        /// Deserialize all selected objects and marks from the macro feature data. The selections
        /// will be active. Note you have to call Commit or Cancel after calling this or the feature
        /// manager tree will be in a rollback state.
        /// </summary>
        protected void LoadSelections()
        {
            if (SwFeatureData != null)
            {
                var result = SwFeatureData.AccessSelections(ModelDoc, null);
                if (!result)
                    throw new Exception("Expected to get true");
                {
                    object objects;
                    object objectTypes;
                    object marks;
                    object drViews;
                    object componentXForms;
                    SwFeatureData.GetSelections3(out objects, out objectTypes, out marks, out drViews, out componentXForms);

                    if (objects == null)
                        return;

                    var objectsArray = (object[]) objects;
                    var typesArray = (swSelectType_e[]) objectTypes;
                    var marksArray = (int[]) marks;
                    //var viewsArray = (IView[]) drViews;
                    //var xformsArray = (IMathTransform[]) componentXForms;

                    var selections = objectsArray.Select((o, i) => 
                    new {o,
                        t = typesArray[i],
                        m = marksArray[i]
                        }).ToList();
                    var selectionsByMark = selections.GroupBy(s => s.m);


                    foreach (var s in selectionsByMark)
                    {
                        var selectionData = SelectionMgr.CreateSelectData();
                        selectionData.Mark = s.Key;
                        var array = s.Select(o=>o.o).ToArray();
                        var count = ModelDoc.Extension.MultiSelect2( ComWangling.ObjectArrayToDispatchWrapper(array), true, selectionData);
                        if (array.Length ==0 && array.Length!=0 )
                        {
                            MessageBox.Show("Unable to select objects");
                        }
                    }
                }
            }
        }


        public void Commit()
        {
            if (State==StateEnum.Insert)
            {
                InsertDefinition(FeatureName, EditBodies, (int) FeatureOptions);
            }
            else
            {
                ModifyDefinition();
            }
        }

        /// <summary>
        /// Cancel editing of the macro feature.
        /// </summary>
        public void Cancel()
        {
            ReleaseSelectionAccess();
        }
    }
}