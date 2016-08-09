using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SolidworksAddinFramework.Wpf;

namespace SolidworksAddinFramework.EditorView
{
    public class ReactiveEditCommand
    {
        public Func<IEditor> CreateEditor { get; }
        public IObservable<bool> CanExecute { get; }
        public IScheduler EditorScheduler { get; }

        public ReactiveEditCommand(
            Func<IEditor> createEditor,
            IObservable<bool> canExecute,
            IScheduler editorScheduler)
        {
            CreateEditor = createEditor;
            CanExecute = canExecute;
            EditorScheduler = editorScheduler;
        }

        public static ReactiveEditCommand Create(Func<IEditor> createEditor)
        {
            return new ReactiveEditCommand(
                createEditor,
                Observable.Return(true),
                UiDispatcherScheduler.Default);
        }
    }

    public static class ReactiveEditCommandExtensions
    {
        public static IReactiveCommand RegisterWith(
            this ReactiveEditCommand command,
            ISerialCommandController serialCommandController)
        {
            return serialCommandController.Register(command);
        }
    }

    public interface ISerialCommandController : INotifyPropertyChanged
    {
        IReactiveCommand Register(ReactiveEditCommand command);
        bool CanEdit { get; }
    }

    /// <summary>
    /// Allows only one edit command to be valid at a time
    /// </summary>
    public class SerialCommandController : ReactiveObject, ISerialCommandController
    {
        [Reactive] public bool CanEdit { get; private set; } = true;

        public IReactiveCommand Register(ReactiveEditCommand commandSpec)
        {
            var canExecute = this.WhenAnyValue(p => p.CanEdit)
                .CombineLatest(commandSpec.CanExecute, (a, b) => a && b)
                .ObserveOnDispatcher();
            var command = ReactiveCommand.Create(canExecute);
            command
                .ObserveOn(commandSpec.EditorScheduler)
                .Subscribe(async _ =>
                {
                    try
                    {
                        var editor = commandSpec.CreateEditor();
                        await ExecuteEditor(editor);
                    }
                    catch (Exception e)
                    {
                        e.Show();
                    }
                });
            return command;
        }

        private async Task ExecuteEditor(IEditor editor)
        {
            try
            {
                CanEdit = false;
                await editor.Edit();
            }
            finally
            {
                CanEdit = true;
            }
        }
    }
}