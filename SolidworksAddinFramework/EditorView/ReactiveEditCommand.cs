using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SolidworksAddinFramework.EditorView
{
    public class ReactiveEditCommand
    {
        public Func<IEditor> CreateEditor { get; }
        public IObservable<bool> CanExecute { get; }
        public IScheduler Scheduler { get; }

        public ReactiveEditCommand(
            Func<IEditor> createEditor,
            IObservable<bool> canExecute,
            IScheduler scheduler)
        {
            CreateEditor = createEditor;
            CanExecute = canExecute;
            Scheduler = scheduler;
        }

        public static ReactiveEditCommand Create(Func<IEditor> createEditor)
        {
            return new ReactiveEditCommand(
                createEditor,
                Observable.Return(true),
                DispatcherScheduler.Current);
        }
    }

    public static class ReactiveEditCommandExtensions
    {
        public static ReactiveCommand<Unit> RegisterWith(
            this ReactiveEditCommand command,
            ISerialCommandController serialCommandController)
        {
            return serialCommandController.Register(command);
        }
    }

    public interface ISerialCommandController : INotifyPropertyChanged
    {
        ReactiveCommand<Unit> Register(ReactiveEditCommand command);
    }

    /// <summary>
    /// Allows only one edit command to be valid at a time
    /// </summary>
    public class SerialCommandController : ReactiveObject, ISerialCommandController
    {
        [Reactive] public bool CanEdit { get; private set; } = true;

        public ReactiveCommand<Unit> Register(ReactiveEditCommand commandSpec)
        {
            var canExecute = this.WhenAnyValue(p => p.CanEdit)
                .CombineLatest(commandSpec.CanExecute, (a, b) => a && b);
            var command = ReactiveCommand.CreateAsyncTask(
                canExecute,
                o => ExecuteEditor(commandSpec.CreateEditor()),
                commandSpec.Scheduler);
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