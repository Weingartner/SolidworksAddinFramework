using System;
using SolidWorks.Interop.sldworks;
using XUnitRemote;

namespace XUnit.Solidworks.Addin
{
    public abstract class SolidWorksSpec : IDisposable
    {
        public static readonly ISldWorks SwApp = (ISldWorks) AppDomain.CurrentDomain.GetData(SwAddin.SwdataKey);
        public void Dispose()
        {
        }

    }
}
