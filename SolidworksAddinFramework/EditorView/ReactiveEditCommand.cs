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
        public IEditor Editor => _CreateEditor();
        public ReactiveCommand<IEditor> Command { get; }
        private readonly Func<IEditor> _CreateEditor;
        private IDisposable _Disposable;

        public ReactiveEditCommand(Func<IEditor> createEditor, ISerialCommandController serialCommandController, IScheduler schedular = null)
        {
            _CreateEditor = createEditor;
            Command = ReactiveCommand.CreateAsyncTask
                (this.WhenAnyValue(p => p.CanEdit)
                    ,_=>Task.FromResult(createEditor())
                    ,schedular ?? UiDispatcherScheduler.Default);
            _Disposable = serialCommandController.Register(this);
        }

        [Reactive]
        public bool CanEdit { get; set; }

        public void Dispose()
        {
            _Disposable.Dispose();
            Command.Dispose();
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
        List<ReactiveEditCommand> Commands = new List<ReactiveEditCommand>();
        public IDisposable Register(ReactiveEditCommand command)
        {
            command.CanEdit = true;
            Commands.Add(command);
            command.Command.Subscribe
                (async editor =>
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
                });

            return Disposable.Create(() => Commands.Remove(command));
        }

        private void EnableCommands()
        {
            Commands.ForEach(c => c.CanEdit = true);
        }

        private void DisableCommands()
        {
            Commands.ForEach(c => c.CanEdit = false);
        }
    }
}