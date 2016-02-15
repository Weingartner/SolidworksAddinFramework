using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;

namespace SwCSharpAddinMF.SWAddin
{
    public abstract class MacroFeatureBase : ISwComFeature
    {
        public IModelDoc2 ModelDoc { get; set; }

        public IFeature SwFeature { get; set; }

        public ISldWorks SwApp { get; set; }

        public object Edit(object app, object modelDoc, object feature)
        {
            SwApp = app as ISldWorks;
            SwFeature = feature as IFeature;
            ModelDoc = modelDoc as IModelDoc2;
            return Edit();
        }

        protected abstract object Edit();
        protected abstract object Security();
        protected abstract object Regenerate();

        public object Regenerate(object app, object modelDoc, object feature)
        {
            SwApp = app as ISldWorks;
            SwFeature = feature as IFeature;
            ModelDoc = modelDoc as IModelDoc2;
            return Regenerate();
        }

        public object Security(object app, object modelDoc, object feature)
        {
            SwApp = app as ISldWorks;
            SwFeature = feature as IFeature;
            ModelDoc = modelDoc as IModelDoc2;
            return Security();
        }
    }

    public static class FeatureManagerExtensions
    {
        public static void InsertMacroFeature<T>(this IFeatureManager featMgr, string featureName, string[] names, int[] types, string[] values, IBody2 editBody, int opts)
        {
            object paramNames = names;
            object paramTypes = types;
            object paramValues = values;

            IFeature MacroFeature = featMgr.InsertMacroFeature3(featureName, typeof (T).FullName, null, (paramNames),
                (paramTypes), (paramValues), null, null, editBody, null, opts);
        }
    }
}