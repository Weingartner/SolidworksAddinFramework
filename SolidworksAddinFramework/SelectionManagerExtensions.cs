using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class SelectionManagerExtensions
    {
        private class ObjectProperties
        {
            public ObjectProperties(int index, swSelectType_e type, int mark)
            {
                Index = index;
                Type = type;
                Mark = mark;
            }

            public int Index { get; }
            public swSelectType_e Type { get; }
            public int Mark { get; }
        }

        private const int AnyMark = -1;
        private const int NoMark = 0;

        private const int StartIndex = 1;

        private static IEnumerable<ObjectProperties> GetSelectedObjectProperties(ISelectionMgr selMgr)
        {
            var count = selMgr.GetSelectedObjectCount();
            return Enumerable.Range(StartIndex, count)
                .Select(i =>
                {
                    var type = (swSelectType_e)selMgr.GetSelectedObjectType3(i, AnyMark);
                    var mark = selMgr.GetSelectedObjectMark(i);
                    return new ObjectProperties(i, type, mark);
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

            return GetSelectedObjectProperties(selMgr)
                .Where(o => filter(o.Type, o.Mark))
                .Select(o => selMgr.GetSelectedObject6(o.Index, AnyMark))
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

        public static IEnumerable<SelectionData> SerializeSelections(this ISelectionMgr selectionMgr)
        {
            return Enumerable
                .Range(1, selectionMgr.GetSelectedObjectCount2(-1))
                .Select(i =>
                {
                    string selectByString;
                    string objectType;
                    int type;
                    double x;
                    double y;
                    double z;
                    var result = selectionMgr.GetSelectionSpecification(
                        i,
                        out selectByString,
                        out objectType,
                        out type,
                        out x,
                        out y, out z);
                    var mark = selectionMgr.GetSelectedObjectMark(i);
                    return new SelectionData(selectByString, objectType, x, y, z, mark);
                });
        }
    }
}