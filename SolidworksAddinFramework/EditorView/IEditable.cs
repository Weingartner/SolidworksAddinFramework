using System.Windows.Input;

namespace SolidworksAddinFramework.EditorView
{
    public interface IEditable
    {
        ICommand EditCommand { get; }
    }
}