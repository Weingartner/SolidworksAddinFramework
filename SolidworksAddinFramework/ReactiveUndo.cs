using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// An undo stack for PropertyManagerPages. The
    /// target object is monitored for change notifications
    /// amd JSON copies are written to a stack. Undo
    /// writes older versions back to the target object.
    /// 
    /// This class is IDisposable because it set's up a
    /// change notification listener on the target
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReactiveUndo<T> : ReactiveObject, IDisposable
        where T : ReactiveObject
    {
        private readonly T _Target;

        private readonly Stack<T> _UndoStack = new Stack<T>();
        private readonly IDisposable _Disposable;
        private bool _Undoing;

        public ReactiveUndo(T target)
        {
            _Target = target;
            Do();
            _Disposable = _Target.Changed
                .DistinctUntilChanged()
                .Subscribe(_ =>
                {
                    if(!_Undoing) Do();
                });
        }

        private void Do()
        {
            if (_UndoStack.Count > 0 && Equals(_UndoStack.Peek(), _Target))
                return;

            _UndoStack.Push(Json.Clone(_Target));

            UpdateCanUndo();
        }

        private bool _CanUndo;
        public bool CanUndo 
        {
            get { return _CanUndo; }
            private set { this.RaiseAndSetIfChanged(ref _CanUndo, value); }
        }

        public void Undo()
        {
            if (!CanUndo)
            {
                return;
            }

            _UndoStack.Pop();
            var json = _UndoStack.Peek();
            _Undoing = true;
            using (Disposable.Create(() => _Undoing = false))
            {
                Json.Copy(json, _Target);
            }

            UpdateCanUndo();
        }

        private void UpdateCanUndo()
        {
            CanUndo = _UndoStack.Count > 1;
        }

        public void Dispose()
        {
            _Disposable.Dispose();
        }
    }
}