using System;
using SolidWorks.Interop.sldworks;
using XUnitRemote.Local;

namespace XUnit.Solidworks.Addin
{
    public abstract class SolidWorksSpec
    {
        protected static ISldWorks SwApp => (ISldWorks)XUnitService.Data[nameof(ISldWorks)]; 
    }
}
