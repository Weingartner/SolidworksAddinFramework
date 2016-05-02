using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ReactiveUI;
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
        where TData : ReactiveObject, new()
        where TMacroFeature : MacroFeatureBase<TMacroFeature, TData>
    {
        // Store PMP in a field so the GC can't collect it
        private PropertyManagerPageBase _EditPage;

        public IModelDoc2 ModelDoc { get; private set; }

        private IFeature _SwFeature;

        protected IMacroFeatureData SwFeatureData { get; private set; }

        public ISldWorks SwApp { get; private set; }
        public TData Database { get; private set; }

        public abstract string FeatureName { get; }

        protected abstract swMacroFeatureOptions_e FeatureOptions { get; }

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
            if (feature != null)
            {
                _SwFeature = (IFeature) feature;
                SwFeatureData = (IMacroFeatureData) _SwFeature.GetDefinition();
                Database = SwFeatureData.Read<TData>();
            }
            else
            {
                Database = new TData();
            }
            ModelDoc = (IModelDoc2) modelDoc;
        }

        private StateEnum State => SwFeatureData == null ? StateEnum.Insert : StateEnum.Edit;

        /// <summary>
        /// Save all selection and parameters and modify the definition of the macro feature
        /// </summary>
        private void ModifyDefinition()
        {
            SaveSelections();
            SwFeatureData.Write(Database);
            _SwFeature.ModifyDefinition(SwFeatureData, ModelDoc, null);
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
        private void SaveSelections()
        {
            var t = ModelDoc.GetMacroFeatureDataSelectionInfo(Database);

            SwFeatureData.SetSelections2(ComWangling.ObjectArrayToDispatchWrapper(t.Item1), t.Item2, t.Item3);
            Debug.Assert(SwFeatureData.GetSelectionCount() == t.Item1.Length);
        }

        /// <summary>
        /// Deserialize all selected objects and marks from the macro feature data. The selections
        /// will be active. Note you have to call Commit or Cancel after calling this or the feature
        /// manager tree will be in a rollback state.
        /// </summary>
        private void LoadSelections()
        {
            if (SwFeatureData == null) return;

            var result = SwFeatureData.AccessSelections(ModelDoc, null);
            if (!result)
            {
                throw new Exception("Can't access selections");
            }
        }


        public void Commit()
        {
            if (State==StateEnum.Insert)
            {
                var editBodies = ModelDoc
                    .GetSelectedObjectsFromModel(Database)
                    .OfType<IBody2>()
                    .ToArray();
                ModelDoc.FeatureManager.InsertMacroFeature<TMacroFeature>(FeatureName, FeatureOptions, Database, editBodies);
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
            SwFeatureData?.ReleaseSelectionAccess();
        }
    }
}