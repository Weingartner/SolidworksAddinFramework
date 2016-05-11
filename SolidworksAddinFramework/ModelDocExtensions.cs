using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SolidworksAddinFramework.Events;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.OpenGl;
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
        public static Task Using(this IModelDoc2 doc, ISldWorks sldWorks, Func<IModelDoc2, Task> run)
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
            var sm = modelDoc
                .SelectionManager
                .DirectCast<ISelectionMgr>();

            filter = filter ?? ((type,mark)=> true);
            return modelDoc
                .UserSelectionPostNotifyObservable()
                .Select(e => sm.GetSelectedObjects(filter));
        }

        public static IDisposable PushSelections(this IModelDoc2 doc, object model)
        {
            var selectionManager = (ISelectionMgr)doc.SelectionManager;
            var revert = selectionManager.DeselectAllUndoable();

            var selections = SelectionDataExtensions.GetSelectionsFromModel(model);

            var selectionMgr = (ISelectionMgr) doc.SelectionManager;
            selections
                .GroupBy(p => p.Mark)
                .Select(p => new { Mark = p.Key, Objects = p.SelectMany(selectionData => selectionData.GetObjects(doc)).ToArray() })
                .Where(p => p.Objects.Length > 0)
                .ForEach(o =>
                {
                    var selectData = selectionMgr.CreateSelectData();
                    selectData.Mark = o.Mark;

                    var count = doc.Extension.MultiSelect2(ComWangling.ObjectArrayToDispatchWrapper(o.Objects), true, selectData);
                });

            return revert;
        }

        public static IEnumerable<object> GetSelectedObjectsFromModel(this IModelDoc2 doc, object model)
        {
            return SelectionDataExtensions.GetSelectionsFromModel(model)
                .SelectMany(data => data.GetObjects(doc));
        }

        public static Tuple<object[], int[], IView[]> GetMacroFeatureDataSelectionInfo(this IModelDoc2 doc, object model)
        {
            var view = (IView) (doc as IDrawingDoc)?.GetFirstView();

            var selections = SelectionDataExtensions.GetSelectionsFromModel(model).ToList();
            var selectedObjects = selections.SelectMany(s => s.GetObjects(doc)).ToArray();
            var marks = selections.SelectMany(s => Enumerable.Repeat(s.Mark, s.ObjectIds.Count)).ToArray();
            var views = selections.SelectMany(s => Enumerable.Repeat(view, s.ObjectIds.Count)).ToArray();
            return Tuple.Create(selectedObjects, marks, views);
        }

        /// <summary>
        /// Doesn't work when intersecting with wire bodies. 
        /// </summary>
        /// <param name="modelDoc"></param>
        /// <param name="ray"></param>
        /// <param name="bodies"></param>
        /// <param name="hitRadius"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static List<RayIntersection> GetRayIntersections(this IModelDoc2 modelDoc, PointDirection3 ray, IBody2[] bodies, double hitRadius, double offset)
        {
            var icount = modelDoc.RayIntersections
                (BodiesIn: bodies
                , BasePointsIn: ray.Point.ToDoubles()
                , VectorsIn: ray.Direction.ToDoubles()
                , Options: (int) (swRayPtsOpts_e.swRayPtsOptsENTRY_EXIT | swRayPtsOpts_e.swRayPtsOptsNORMALS |
                                  swRayPtsOpts_e.swRayPtsOptsTOPOLS | swRayPtsOpts_e.swRayPtsOptsUNBLOCK)
                , HitRadius: hitRadius
                , Offset: offset);
            var result = modelDoc.GetRayIntersectionsPoints().CastArray<double>();

            const int fields = 9;
            return Enumerable.Range(0, icount)
                .Select(i =>
                {
                    var baseOffset = i * fields;

                    var bodyIndex = result[baseOffset + 0];
                    var rayIndex = result[baseOffset + 1];
                    var intersectionType = result[baseOffset + 2];
                    var x = result[baseOffset + 3];
                    var y = result[baseOffset + 4];
                    var z = result[baseOffset + 5];
                    var nx = result[baseOffset + 6];
                    var ny = result[baseOffset + 7];
                    var nz = result[baseOffset + 8];

                    return new RayIntersection(
                        bodies[(int)bodyIndex],
                        (int)rayIndex,
                        (swRayPtsResults_e)intersectionType,
                        new [] { x, y, z }.ToVector3(),
                        new[] { nx, ny, nz }.ToVector3()
                        );
                }).ToList();
        }

        public class RayIntersection
        {
            public RayIntersection(IBody2 body, int rayIndex, swRayPtsResults_e intersectionType, Vector3 hitPoint, Vector3 normals)
            {
                Body = body;
                RayIndex = rayIndex;
                IntersectionType = intersectionType;
                HitPoint = hitPoint;
                Normals = normals;
            }

            public IBody2 Body { get; }
            public int RayIndex { get; }
            public swRayPtsResults_e IntersectionType { get; }
            public Vector3 HitPoint { get; }
            public Vector3 Normals { get; }

        }

        /// <summary>
        /// From a given X,Y screen coordinate return the model
        /// coordinates and the direction of looking.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static PointDirection3 ScreenToView(this IModelDoc2 doc, int x, int y)
        {
            var math = SwAddinBase.Active.Math;
            var view = doc.ActiveView.DirectCast<IModelView>();
            var t = view.Transform.Inverse().DirectCast<MathTransform>();

            var eye = math.Point(new[] {x, y, 0.0});

            var look = math.ZAxis().DirectCast<MathVector>();

            eye = eye.MultiplyTransformTs(t);
            look = look.MultiplyTransformTs(t);

            return new PointDirection3(Vector3Extensions.ToVector3(eye), look.ToVector3().Unit());
        }

        public static Vector2 ViewToScreen(this IModelDoc2 doc, Vector3 point)
        {
            var math = SwAddinBase.Active.Math;
            var view = doc.ActiveView.DirectCast<IModelView>();
            var t = view.Transform.DirectCast<MathTransform>();
            var mathPoint = point.ToSwMathPoint(math);
            mathPoint = mathPoint.MultiplyTransformTs(t);
            var v3 = mathPoint.ToVector3();
            return new Vector2(v3.X, v3.Y);
        }
    }
}
