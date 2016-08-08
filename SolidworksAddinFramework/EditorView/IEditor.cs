using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SolidworksAddinFramework.EditorView
{
    public interface IEditor : IDisposable
    {
        Task<bool> Edit();
    }
}