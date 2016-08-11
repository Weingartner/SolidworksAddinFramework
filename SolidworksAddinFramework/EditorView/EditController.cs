using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SolidworksAddinFramework.EditorView
{
    /// <summary>
    /// Allows only one edit command to be valid at a time
    /// </summary>
    public class EditController : ReactiveObject, ISerialCommandController
    {
        [Reactive] public Option<object> Editing { get; private set; } = None;

        public IReactiveCommand Register(ReactiveEditCommand commandSpec, IScheduler scheduler)
        {
            var canExecute = this.WhenAnyValue(p => p.Editing)
                .CombineLatest(commandSpec.CanExecute, (a, b) => a.IsNone && b)
                .ObserveOn(scheduler);

            var command = ReactiveCommand.Create(canExecute);
            command
                .ObserveOn(commandSpec.EditorScheduler)
                .Subscribe(async _ =>
                {
                    try
                    {
                        var editor = commandSpec.CreateEditor();
                        var o = commandSpec.Editable;
                        await ExecuteEditor(editor,o);
                    }
                    catch (Exception e)
                    {
                        e.Show();
                    }
                });
            return command;
        }


        private async Task ExecuteEditor(IEditor editor, object o)
        {
            try
            {
                Editing = Some(o);
                await editor.Edit();
            }
            finally
            {
                Editing = None;
            }
        }
    }
}