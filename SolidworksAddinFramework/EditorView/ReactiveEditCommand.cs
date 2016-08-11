using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using LanguageExt;
using ReactiveUI;
using SolidworksAddinFramework.Wpf;

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
        public IScheduler EditorScheduler { get; }

        public ReactiveEditCommand(
            Func<IEditor> createEditor,
            IObservable<bool> canExecute,
            IScheduler editorScheduler,
            object editable)
        {
            Editable = editable;
            CreateEditor = createEditor;
            CanExecute = canExecute.ObserveOnDispatcher();
            EditorScheduler = editorScheduler;
        }

        public static ReactiveEditCommand Create(Func<IEditor> createEditor, IScheduler schedular, object editable)
        {
            return new ReactiveEditCommand(
                createEditor,
                Observable.Return(true)
                ,schedular
                ,editable);
        }
    }

    public static class ReactiveEditCommandExtensions
    {
        public static IReactiveCommand RegisterWith(this ReactiveEditCommand command, ISerialCommandController serialCommandController, IScheduler schedular)
        {
            return serialCommandController.Register(command, schedular);
        }
    }

    public interface ISerialCommandController : INotifyPropertyChanged
    {
        IReactiveCommand Register(ReactiveEditCommand command, IScheduler wpfScheduler);
        Option<object> Editing { get; }
    }
}