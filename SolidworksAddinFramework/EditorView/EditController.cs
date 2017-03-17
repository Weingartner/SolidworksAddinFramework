using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Monads;
using Unit = System.Reactive.Unit;

namespace SolidworksAddinFramework.EditorView
{
    /// <summary>
    /// Allows only one edit command to be valid at a time
    /// </summary>
    public class EditController : ReactiveObject, ISerialCommandController
    {
        [Reactive] public Option<object> Editing { get; private set; } = None;

        public ReactiveCommand Register(ReactiveEditCommand commandSpec)
        {
            var canExecute = this.WhenAnyValue(p => p.Editing)
                                 .CombineLatest(commandSpec.CanExecute, (a, b) => a.IsNone && b);

            var foo = Unit.Default;
            var command = ReactiveCommand.Create(canExecute:canExecute, execute:
                async () =>
                {
                    try
                    {
                        await ExecuteEditor(commandSpec);
                    }
                    catch (Exception e)
                    {
                        e.Show();
                    }
                });
            return command;
        }


        private async Task ExecuteEditor(ReactiveEditCommand commandSpec)
        {
            var editor = commandSpec.CreateEditor();
            try
            {
                Editing = Some(commandSpec.Editable);
                await editor.Edit();
            }
            finally
            {
                Editing = None;
            }
        }
    }
}