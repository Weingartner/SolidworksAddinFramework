using System.Threading.Tasks;
using ReactiveUI;

namespace SolidworksAddinFramework.EditorView
{
    public class EditorEmpty : ReactiveObject, IEditor
    {
        public void Dispose()
        {
        }

        public bool IsEditing => false;
        public Task<bool> Edit()
        {
            return Task.FromResult(true);
        }
    }
}