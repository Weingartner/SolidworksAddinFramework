using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// This page works with a copy of the live data. Changes are
    /// only propogated to the live data when OK is pressed. An
    /// undo stack is also available. The undo stack is cleared
    /// when the data is commited.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CommitableUndoablePropertyManagerPage<T> : ReactiveObjectPropertyManagerPageBase<T>
        where T : ReactiveUI.ReactiveObject
    {
        private readonly ReactiveTransaction<T> _Transaction;
        private ReactiveUndo<T> _ReactiveUndo;

        private CommitableUndoablePropertyManagerPage(string name
            , IEnumerable<swPropertyManagerPageOptions_e> optionsE
            , ISldWorks swApp
            , IModelDoc2 modelDoc
            , ReactiveTransaction<T> transaction)
            : base(name, optionsE.Concat(new[] { swPropertyManagerPageOptions_e.swPropertyManagerOptions_UndoButton }), swApp, modelDoc, transaction.Data)
        {
            _Transaction = transaction;
        }

        protected CommitableUndoablePropertyManagerPage
            (string name
                , IEnumerable<swPropertyManagerPageOptions_e> optionsE
                , ISldWorks swApp
                , IModelDoc2 modelDoc
                , T data) : this(name, optionsE, swApp, modelDoc, new ReactiveTransaction<T>(data))
        {
            this.WhenAnyValue(p => p.IsValid)
                .Where(_ => Page != null)
                .Subscribe(isValid => Page.EnableButton((int)swPropertyManagerPageButtons_e.swPropertyManagerPageButton_Ok, isValid));
        }

        protected override void OnShow()
        {
            _ReactiveUndo = new ReactiveUndo<T>(_Transaction.Data);
            var d0 = _ReactiveUndo.WhenAnyValue(p => p.CanUndo)
                .Subscribe(p => Page.EnableButton((int)swPropertyManagerPageButtons_e.swPropertyManagerPageButton_Undo, p));
            DisposeOnClose(d0);
            DisposeOnClose(_ReactiveUndo);
            DisposeOnClose(Disposable.Create(() => _ReactiveUndo = null));
        }

        protected override void OnCancel()
        {
            _Transaction.Rollback();
        }

        protected override void OnCommit()
        {
            _Transaction.Commit();
        }

        public override void OnUndo()
        {
            _ReactiveUndo.Undo();
            base.OnUndo();
        }

        bool _IsValid;
        public bool IsValid 
        {
            get { return _IsValid; }
            set { this.RaiseAndSetIfChanged(ref _IsValid, value); }
        }
    }
}