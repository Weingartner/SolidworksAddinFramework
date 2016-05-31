using System;
using System.Collections.Generic;
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
        public T Clone { get; }

        private readonly Stack<string> _UndoStack = new Stack<string>();
        private readonly IDisposable _Disposable;
        private bool _Undoing;

        public ReactiveUndo(T target)
        {
            Clone = target;
            Do();
            _Disposable = target.Changed.Subscribe(_ =>
            {
                if(!_Undoing) Do();
            });
        }

        private void Do()
        {
            _UndoStack.Push(Clone.ToJson());

            UpdateCanUndo();
        }

        bool _CanUndo;
        public bool CanUndo 
        {
            get { return _CanUndo; }
            set { this.RaiseAndSetIfChanged(ref _CanUndo, value); }
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
            Json.Copy(json, Clone);
            _Undoing = false;

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