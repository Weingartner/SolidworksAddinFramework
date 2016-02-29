using System;
using System.Collections.Generic;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class SelectionManagerExtensions
    {
        /// <summary>
        /// Get selected objects filtered by type and mark
        /// </summary>
        /// <param name="selMgr"></param>
        /// <param name="filter">(type,mark)=>bool</param>
        /// <returns></returns>
        public static IEnumerable<object> GetSelectedObjects(this ISelectionMgr selMgr, Func<swSelectType_e, int, bool> filter)
        {
            var count = selMgr.GetSelectedObjectCount();
            for (var i = 1; i <= count; i++)
            {
                var type = (swSelectType_e) selMgr.GetSelectedObjectType3(i, -1);
                var mark = selMgr.GetSelectedObjectMark(i);
                if (filter(type, mark))
                    yield return selMgr.GetSelectedObject6(i,-1);
            }
        }
         
    }
}