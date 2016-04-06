using System;
using SolidWorks.Interop.sldworks;
using XUnitRemote;

namespace XUnit.Solidworks.Addin
{
    public abstract class SolidWorksSpec : IDisposable
    {
        public static ISldWorks SwApp => (ISldWorks)XUnitService.Data[nameof(ISldWorks)]; 
        public void Dispose()
        {
        }

    }
}
