using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Weingartner.ReactiveCompositeCollections.Annotations;

namespace SolidworksAddinFramework.EditorView
{
    public class ReactiveEditCommand : ReactiveObject, IDisposable
    {
        public ReactiveCommand<IEditor> Command { get; }
        private readonly CompositeDisposable _Disposable = new CompositeDisposable();

        public ReactiveEditCommand(
            Func<IEditor> createEditor,
            ISerialCommandController serialCommandController,
            IScheduler scheduler)
        {
            Command = ReactiveCommand.CreateAsyncTask(
                    this.WhenAnyValue(p => p.CanEdit),
                    _ => Task.FromResult(createEditor()),
                    scheduler
                )
                .DisposeWith(_Disposable);

            serialCommandController
                .Register(this)
                .DisposeWith(_Disposable);
        }

        [Reactive]
        public bool CanEdit { get; set; }

        public void Dispose()
        {
            _Disposable.Dispose();
            Command.Dispose();
        }

        public static ReactiveEditCommand Create(
            Func<IEditor> createEditor,
            ISerialCommandController serialCommandController)
        {
            return new ReactiveEditCommand(createEditor, serialCommandController, DispatcherScheduler.Current);
        }
    }

    public interface ISerialCommandController
    {
        IDisposable Register(ReactiveEditCommand command);
    }

    /// <summary>
    /// Allows only one edit command to be valid at a time
    /// </summary>
    public class SerialCommandController : ISerialCommandController
    {
        private readonly List<ReactiveEditCommand> _Commands = new List<ReactiveEditCommand>();
        public IDisposable Register(ReactiveEditCommand command)
        {
            var d = new CompositeDisposable();

            command.CanEdit = true;
            AddCommand(command)
                .DisposeWith(d);

            command.Command
                .Subscribe(async editor => await ExecuteEditor(editor))
                .DisposeWith(d);

            return d;
        }

        private IDisposable AddCommand(ReactiveEditCommand command)
        {
            _Commands.Add(command);
            return Disposable.Create(() => _Commands.Remove(command));
        }

        private async Task ExecuteEditor(IEditor editor)
        {
            try
            {
                DisableCommands();
                await editor.Edit();
            }
            finally
            {
                EnableCommands();
            }
        }

        private void EnableCommands()
        {
            _Commands.ForEach(c => c.CanEdit = true);
        }

        private void DisableCommands()
        {
            _Commands.ForEach(c => c.CanEdit = false);
        }
    }
}