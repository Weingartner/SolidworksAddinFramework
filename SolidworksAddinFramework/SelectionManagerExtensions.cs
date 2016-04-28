using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public class ObjectSelection
    {
        public ObjectSelection(object @object, swSelectType_e type, int mark, int index)
        {
            Object = @object;
            Index = index;
            Type = type;
            Mark = mark;
        }

        public object Object { get; set; }
        public int Index { get; }
        public swSelectType_e Type { get; }
        public int Mark { get; }
    }

    public static class SelectionManagerExtensions
    {
        private const int AnyMark = -1;
        private const int NoMark = 0;

        private const int StartIndex = 1;

        public static IEnumerable<ObjectSelection> GetObjectSelections(this ISelectionMgr selMgr)
        {
            var count = selMgr.GetSelectedObjectCount2(AnyMark);
            return Enumerable.Range(StartIndex, count)
                .Select(index =>
                {
                    var type = (swSelectType_e)selMgr.GetSelectedObjectType3(index, AnyMark);
                    var mark = selMgr.GetSelectedObjectMark(index);
                    var obj = selMgr.GetSelectedObject6(index, mark);
                    return new ObjectSelection(obj, type, mark, index);
                });
        }

        /// <summary>
        /// Get selected objects filtered by type and mark
        /// </summary>
        /// <param name="selMgr"></param>
        /// <param name="filter"> <![CDATA[(type,mark)=>bool]]> if null then the default is true </param>
        /// <returns></returns>
        public static IReadOnlyList<object> GetSelectedObjects(this ISelectionMgr selMgr, Func<swSelectType_e, int, bool> filter)
        {
            filter = filter ?? ((type,mark)=> true);

            return selMgr.GetObjectSelections()
                .Where(o => filter(o.Type, o.Mark))
                .Select(o => o.Object)
                .ToList();
        }

        public static void DeselectAll(this ISelectionMgr selMgr)
        {
            Enumerable.Repeat(Unit.Default, selMgr.GetSelectedObjectCount())
                .ForEach(o => selMgr.DeSelect2(StartIndex, AnyMark));
        }

        public static IDisposable DeselectAllUndoable(this ISelectionMgr selMgr)
        {
            selMgr.SuspendSelectionList();
            return Disposable.Create(selMgr.ResumeSelectionList);
        }
    }
}