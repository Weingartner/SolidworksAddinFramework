using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class ModelDocExtensions
    {
        public static IBody2[] GetBodiesTs(this IModelDoc2 doc, swBodyType_e type = swBodyType_e.swSolidBody,
            bool visibleOnly = false)
        {
            var part = (IPartDoc) doc;
            var objects = (object[]) part.GetBodies2((int) type, visibleOnly);
            return objects?.Cast<IBody2>().ToArray() ?? new IBody2[0];
        }

        public static IDisposable CloseDisposable(this IModelDoc2 @this)
        {
            return Disposable.Create(@this.Close);
        }

        public static void Using(this IModelDoc2 doc, ISldWorks sldWorks, Action<IModelDoc2> run)
        {
            doc.Using(m => sldWorks.CloseDoc(doc.GetTitle()), run);
        }
        public static Task Using(this IModelDoc2 doc, ISldWorks sldWorks, Func<IModelDoc2,Task> run)
        {
            return doc.Using(m => sldWorks.CloseDoc(doc.GetTitle()), run);
        }


        /// <summary>
        /// Get all reference planes from the model
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static IEnumerable<IRefPlane> GetPlanes(this IModelDoc2 doc)
        {
            return doc.FeatureManager
                .GetFeatures(false)
                .CastArray<IFeature>()
                .Select(f => f.GetSpecificFeature2() as IRefPlane);
        }
        public static IObservable<IReadOnlyList<object>> SelectionObservable(this IModelDoc2 modelDoc, 
            Func<swSelectType_e, int, bool> filter = null)
        {
            filter = filter ?? ((type,mark)=> true);
            return
            (modelDoc as PartDoc)?.SelectionObservable(filter)
                ??
            (modelDoc as DrawingDoc)?.SelectionObservable(filter)
                ??
            (modelDoc as AssemblyDoc)?.SelectionObservable(filter);

        }


        #region Convet modeldoc / partdoc events to observables

        public static IObservable<IReadOnlyList<object>> SelectionObservable(this PartDoc partDoc, 
            Func<swSelectType_e, int, bool> filter = null)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var modelDoc = (IModelDoc2) partDoc;
            var sm = (ISelectionMgr) modelDoc.SelectionManager;
            filter = filter ?? ((type,mark)=> true);

            return ObservableExtensions.FromEvent
                (h => partDoc.UserSelectionPostNotify += h,
                    h => partDoc.UserSelectionPostNotify -= h,
                    d => new DPartDocEvents_UserSelectionPostNotifyEventHandler(d))
                .Select(u => sm.GetSelectedObjects(filter));
        }

        public static IObservable<IReadOnlyList<object>> SelectionObservable(this DrawingDoc drawingDoc, 
            Func<swSelectType_e, int, bool> filter = null)
        {
            var modelDoc = (IModelDoc2) drawingDoc;
            var sm = (ISelectionMgr) modelDoc.SelectionManager;
            filter = filter ?? ((type,mark)=> true);

            return ObservableExtensions.FromEvent
                (h => drawingDoc.UserSelectionPostNotify += h,
                h => drawingDoc.UserSelectionPostNotify -= h,
                d => new DDrawingDocEvents_UserSelectionPostNotifyEventHandler(d) )
                .Select(u => sm.GetSelectedObjects(filter));
        }
        public static IObservable<IReadOnlyList<object>> SelectionObservable(this AssemblyDoc assemblyDoc, 
            Func<swSelectType_e, int, bool> filter)
        {
            var modelDoc = (IModelDoc2) assemblyDoc;
            var sm = (ISelectionMgr) modelDoc.SelectionManager;
            return ObservableExtensions.FromEvent
                (h => assemblyDoc.UserSelectionPostNotify += h,
                h => assemblyDoc.UserSelectionPostNotify -= h,
                d => new DAssemblyDocEvents_UserSelectionPostNotifyEventHandler(d) )
                .Select(u => sm.GetSelectedObjects(filter));
        }

        #endregion
    }
}
