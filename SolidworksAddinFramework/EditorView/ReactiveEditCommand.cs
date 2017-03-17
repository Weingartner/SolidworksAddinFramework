using System;
using System.ComponentModel;
using System.Reactive.Linq;
using LanguageExt;
using ReactiveUI;

namespace SolidworksAddinFramework.EditorView
{
    public class ReactiveEditCommand
    {
        /// <summary>
        /// The editable object the editor will edit
        /// </summary>
        public object Editable { get; }
        public Func<IEditor> CreateEditor { get; }
        public IObservable<bool> CanExecute { get; }

        public ReactiveEditCommand(
            Func<IEditor> createEditor,
            IObservable<bool> canExecute,
            object editable)
        {
            Editable = editable;
            CreateEditor = createEditor;
            CanExecute = canExecute.ObserveOnDispatcher();
        }

        public static ReactiveEditCommand Create(Func<IEditor> createEditor, object editable)
        {
            return new ReactiveEditCommand(
                createEditor,
                Observable.Return(true),editable);
        }
    }

    public static class ReactiveEditCommandExtensions
    {
        public static ReactiveCommand RegisterWith(this ReactiveEditCommand command, ISerialCommandController serialCommandController)
        {
            return serialCommandController.Register(command);
        }
    }

    public interface ISerialCommandController : INotifyPropertyChanged
    {
        ReactiveCommand Register(ReactiveEditCommand command);
        Option<object> Editing { get; }
    }
}