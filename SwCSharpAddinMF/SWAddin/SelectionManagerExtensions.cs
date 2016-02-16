using System;
using System.Collections.Generic;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SwCSharpAddinMF.SWAddin
{
    public static class SelectionManagerExtensions
    {
        public static IEnumerable<object> GetSelectedObjects(this ISelectionMgr selMgr, Func<swSelectType_e, int, bool> filter)
        {
            var count = selMgr.GetSelectedObjectCount();
            for (int i = 1; i <= count; i++)
            {
                var type = (swSelectType_e) selMgr.GetSelectedObjectType3(i, -1);
                var mark = selMgr.GetSelectedObjectMark(i);
                if (filter(type, mark))
                    yield return selMgr.GetSelectedObject6(i,-1);
            }
        }
         
    }
}