using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SolidworksAddinFramework.EditorView
{
    /// <summary>
    /// Allows only one edit command to be valid at a time
    /// </summary>
    public class SerialCommandController : ReactiveObject, ISerialCommandController
    {
        [Reactive] public bool CanEdit { get; private set; } = true;

        public IReactiveCommand Register(ReactiveEditCommand commandSpec, IScheduler scheduler)
        {
            var canExecute = this.WhenAnyValue(p => p.CanEdit)
                .CombineLatest(commandSpec.CanExecute, (a, b) => a && b)
                .ObserveOn(scheduler);

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