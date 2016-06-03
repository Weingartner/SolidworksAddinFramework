using System;
using System.Collections.Generic;
using System.Drawing;
using LanguageExt;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public abstract class ReactiveObjectPropertyManagerPageBase<T> : PropertyManagerPageBase
        where T : ReactiveUI.ReactiveObject
    {
        public T Data { get; }

        protected ReactiveObjectPropertyManagerPageBase
            (string name
                , IEnumerable<swPropertyManagerPageOptions_e> optionsE
                , ISldWorks swApp
                , IModelDoc2 modelDoc
                , T data) : base(name, optionsE, swApp, modelDoc)
        {
            Data = data;
        }

        public sealed override void Show()
        {
            base.Show();
            OnShow();
        }

        protected abstract void OnShow();

        protected override IDisposable PushSelections()
        {
            return ModelDoc.PushSelections(Data);
        }

        protected override void OnClose(swPropertyManagerPageCloseReasons_e reason)
        {
            switch (reason)
            {
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Cancel:
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_UnknownReason:
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_UserEscape:
                    OnCancel();
                    break;
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay:
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Apply:
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Closed: // renders as green tick, so I guess it means "save"
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_ParentClosed: // don't know what this is, maybe it applies to `swPropertyManagerPageOptions_e .swPropertyManagerOptions_MultiplePages`
                default:
                    OnCommit();
                    break;
            }
        }

        protected abstract void OnCommit();

        protected abstract void OnCancel();
    }
}