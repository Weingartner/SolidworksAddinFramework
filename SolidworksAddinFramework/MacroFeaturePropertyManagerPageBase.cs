using System;
using System.Collections.Generic;
using ReactiveUI;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public abstract class MacroFeaturePropertyManagerPageBase<TMacroFeature, TData> : PropertyManagerPageBase
        where TData : ReactiveObject, new()
        where TMacroFeature : MacroFeatureBase<TMacroFeature,TData>
    {
        protected MacroFeaturePropertyManagerPageBase(IEnumerable<swPropertyManagerPageOptions_e> options,TMacroFeature macroFeature) 
            : base(macroFeature.FeatureName, options, macroFeature.SwApp, macroFeature.ModelDoc)
        {
            MacroFeature = macroFeature;
        }

        public TMacroFeature MacroFeature { get; private set; }
        protected override void AddSelections()
        {
            ModelDoc.AddSelections(MacroFeature.Database);
        }

        protected override void OnClose(swPropertyManagerPageCloseReasons_e reason)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                MacroFeature.Commit();
            }
            else if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Cancel)
            {
                MacroFeature.Cancel();
            }
        }
    }
}