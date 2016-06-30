using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Xml;
using Accord.Math.Optimization;
using LanguageExt;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class BodyExtensions
    {
        public static IBody2 Add(this IEnumerable<IBody2> bodies)
        {
            return bodies.Aggregate((a, acc) => a.Add(acc).Bodies[0]);
        }

        /// <summary>
        /// Result for OperationsTs call.
        /// </summary>
        public class OperationsResult
        {
            /// <summary>
            /// If 0 then there is no error
            /// </summary>
            public int Error { get; }

            /// <summary>
            /// If error == 0 then this contains the resulting bodies from the operations
            /// </summary>
            public IBody2[] Bodies { get; }

            public OperationsResult(int error, IBody2[] bodies)
            {
                Error = error;
                Bodies = bodies;
            }
        }

        #region boolean operations
        /// <summary>
        /// Perform Add, Cut, Intersect operations on solid bodies. 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="type"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static OperationsResult OperationsTs(this IBody2 body, swBodyOperationType_e type, IBody2 tool)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            if (tool == null) throw new ArgumentNullException(nameof(tool));

            int error;
            var objects = (object[]) body.Operations2((int) type, tool, out error);
            if(objects==null)
                return new OperationsResult(error, new IBody2[] {});

            var bodies = error == 0 ?  objects.Cast<IBody2>().ToArray() : new IBody2[] {};
            return new OperationsResult(error, bodies);
        }

        public static OperationsResult Cut(this IBody2 body , IBody2 tool)
        {
            return body.OperationsTs(swBodyOperationType_e.SWBODYCUT, tool);
        } 
        public static OperationsResult Add(this IBody2 body , IBody2 tool)
        {
            return body.OperationsTs(swBodyOperationType_e.SWBODYADD, tool);
        } 
        public static OperationsResult Intersect(this IBody2 body , IBody2 tool)
        {
            return body.OperationsTs(swBodyOperationType_e.SWBODYINTERSECT, tool);
        }
        #endregion

        public static void HideAll(this IEnumerable<IBody2> bodies, IModelDoc2 doc)
        {
            foreach (var body in bodies)
            {
                body.Hide(doc);
            }
        }

        public static void DisplayTs(this IBody2 body ,IModelDoc2 doc, Color? c = null, swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
            c = c ?? Color.Yellow;
            var colorref = ColorTranslator.ToWin32(c.Value);
            body.Display3(doc, colorref, (int) opt);
        }

        public static void DisplayAll(this IEnumerable<IBody2> bodies, IModelDoc2 doc, Color c, swTempBodySelectOptions_e opt)
        {
            foreach (var body in bodies)
            {
                body.DisplayTs(doc, c, opt);
            }
        }

        public static bool ApplyTransform(this IBody2 body, Matrix4x4 t)
        {
            var transform = SwAddinBase.Active.Math.ToSwMatrix(t);
            return body.ApplyTransform(transform);
        }

        /// <summary>
        /// Return the bounding box of the solid renderable.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Range3Single GetBodyBoxTs(this IBody2 body)
        {
            var box = (double[]) body.GetBodyBox();
            return new Range3Single(
                new Vector3((float) box[0], (float) box[1], (float) box[2]), 
                new Vector3((float) box[3], (float) box[4], (float) box[5]));
        }

        public static bool GetDistance(this IBody2 body0, IBody2 body1, out double[] p0, out double []p1)
        {
            var faces0 = body0.GetFaces().CastArray<IFace2>();
            var faces1 = body1.GetFaces().CastArray<IFace2>();
            var distances = GetDistances(faces0, faces1);

            p0 = null;
            p1 = null;


            var shortest = distances.MinBy(t => MathPointExtensions.Distance(t.Item1, t.Item2));
            if (shortest.Count == 0)
                return false;

            p0 = shortest[0].Item1;
            p1 = shortest[0].Item2;

            return true;
        }


        public static IEnumerable<Tuple<double[], double[]>> GetDistances(IEnumerable<IFace2> entities0, IEnumerable<IFace2> entities1)
        {
            var entities1List = entities1 as IList<IFace2> ?? entities1.ToList();

            foreach (var entity0 in entities0)
            {
                foreach (var entity1 in entities1List)
                {
                    double[] posacast;
                    double[] posbcast;
                    if(entity0.GetDistance(entity1, out posacast, out posbcast))
                        yield return Tuple.Create( posacast, posbcast);
                    else
                    {
                        throw new Exception("Unable to get distance between faces");
                    }
                }
            }
        }


        /// <summary>
        /// Get the edges formed by intersection both bodies together. The bodies are first copied 
        /// before intersecting so that the input bodies are not modified.
        /// </summary>
        /// <param name="toolBody"></param>
        /// <param name="cuttingPlane"></param>
        /// <returns></returns>
        public static List<IEdge> GetIntersectionEdgesNonDestructive(this IBody2 toolBody, IBody2 cuttingPlane)
        {
            toolBody = (IBody2) toolBody.Copy();
            cuttingPlane = (IBody2) cuttingPlane.Copy();
            var enumerable = ((object[]) toolBody.GetIntersectionEdges(cuttingPlane)).Cast<IEdge>().ToList();
            return enumerable;
        }

        public static IBody2 CopyTs(this IBody2 tool)
        {
            return (IBody2) tool.Copy();
        }

        //public static List<ICurve> GetXzCrossSectionCurves(this IBody2 body)
        //{
        //    var modeler = SwAddinBase.Active.Modeler;
        //    var yZPlane = ((ISurface) modeler.CreatePlanarSurface2(new[] {0, 0, 0.0}, new[] {1.0, 0, 0}, new[] {0, 0, 1.0})).ToSheetBody();

        //    var result = body.Cut(yZPlane);
        //    Debug.Assert(result.Error == 0);

        //    var zXPlane = ((ISurface) modeler.CreatePlanarSurface2(new[] {0, 0, 0.0}, new[] {0, 1.0, 0}, new[] {0, 0, 1.0})).ToSheetBody();
        //    var edges = body.GetIntersectionEdgesNonDestructive(zXPlane);

        //    return edges
        //        .Select(e => e.GetCurveTs())
        //        .Where(c => !(Math.Abs(c.StartPoint().X) < 1e-6 & Math.Abs(c.EndPoint().X) < 1e-6))
        //        .ToList();
        //            {
        //                var result = tgt.Cut(tool);
        //                if (result.Error != 0)
        //                    return new[] {tgt};
        //                    //throw new Exception("Tool was unable to cut");
        //                return result.Bodies;
        //            })
        //            .ToList();
        //    }
        //    return targets;
        //}

        public static Option<Curve> GetIntersectionCurve(this IBody2 toolBody, IBody2 cuttingPlane, IModeler modeler)
        {
            var innerCurves =
                toolBody.GetIntersectionEdgesNonDestructive(cuttingPlane)
                    .Buffer(2, 2)
                    .Select(b => (ICurve) b[0].GetCurve())
                    .Select(c=>(ICurve)c.Copy())
                    .ToArray();

            if (innerCurves.Length <= 0)
                return Prelude.None;

            var curve = modeler.MergeCurves(innerCurves);
            return Prelude.Optional(curve);
        }
    }
    

    public static class DisplayTransaction
    {
        public static IDisposable DisplayUndoable(this IEnumerable<IBody2> bodies, IModelDoc2 doc,
            System.Drawing.Color? c = null,
            swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
                var view = (IModelView)doc.ActiveView;
                using (view.DisableGraphicsUpdate())
                {
                    var d = bodies.Select(toolBody => toolBody.DisplayUndoable(doc, c)).ToCompositeDisposable();
                    return Disposable.Create(() =>
                    {
                        using (view.DisableGraphicsUpdate())
                        {
                            d.Dispose();
                        }
                    });
                }

        }

        public static BodyMaterials GetMaterial(this IBody2 body) => new BodyMaterials(body);
        public static Color GetColor(this IBody2 body) => body.GetMaterial().Color;
        public static void SetColor(this IBody2 body, Color color)
        {
            body.GetMaterial().Color = color;

        }

        public static IDisposable DisplayUndoable
            (this IRenderable renderable, IModelDoc2 doc, int layer = 0)
        {
            return OpenGlRenderer.DisplayUndoable(renderable, doc, layer);
        }

        /// <summary>
        /// Uses Display3 to render the object. This is slow for animation. Better to create a Mesh and then render it. This
        /// will use OpenGL directly.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="doc"></param>
        /// <param name="c"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static IDisposable DisplayUndoable(this IBody2 body, IModelDoc2 doc, Color? c = null, swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
            body.DisplayTs(doc, c, opt);
            return Disposable.Create(() => body.Hide(doc));
        }

        public static IDisposable DisplayBodiesUndoable(this IEnumerable<IBody2> bodies, IModelDoc2 doc, Color? c = null,
            swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
            return new CompositeDisposable(bodies.Select(b=>b.DisplayUndoable(doc,c,opt)));
        }

        public static IDisposable HideBodyUndoable(this IBody2 body)
        {
            body.HideBody(true);
            return Disposable.Create(() =>
            {
                body.HideBody(false);
            });
        }
        public static IDisposable HideBodiesUndoable(this IEnumerable<IBody2> bodies)
        {
            return new CompositeDisposable(bodies.Select(b=>b.HideBodyUndoable()));
        }

    }
}