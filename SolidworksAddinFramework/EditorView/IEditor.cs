using System;
using System.Threading.Tasks;

namespace SolidworksAddinFramework.EditorView
{
    public interface IEditor : IDisposable
    {
        Task<bool> Edit();
    }
}