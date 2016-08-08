using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Subjects;
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
                , IEnumerable<swPropertyManagerPageOptions_e> optionsE, IModelDoc2 modelDoc
                , T data) : base(name, optionsE, modelDoc)
        {
            Data = data;
        }

        public sealed override void Show()
        {
            base.Show();
            OnShow();
        }

        protected abstract void OnShow();

        protected override void AddSelections()
        {
            ModelDoc.AddSelectionsFromModel(Data);
        }
    }
}