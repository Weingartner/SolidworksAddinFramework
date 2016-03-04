using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class BodyExtensions
    {

        public class TwoPointRange
        {
            public double[] P0 { get; }

            public double[] P1 { get; }

            public double[] Center => new double[]
            {
                (P0[0]+P1[0])/2,
                (P0[1]+P1[1])/2,
                (P0[2]+P1[2])/2
            };

            public TwoPointRange(double[] p0, double[] p1)
            {
                P0 = p0;
                P1 = p1;
            }

            public double XMin => Math.Min(P0[0], P1[0]);
            public double XMax => Math.Max(P0[0], P1[0]);

            public double YMin => Math.Min(P0[1], P1[1]);
            public double YMax => Math.Max(P0[1], P1[1]);
            public double ZMin => Math.Min(P0[2], P1[2]);
            public double ZMax => Math.Max(P0[2], P1[2]);

            public double[] XRange => new[] {XMin, XMax};
            public double[] YRange => new[] {YMin, YMax};
            public double[] ZRange => new[] {ZMin, ZMax};
        }

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

        public static void DisplayTs(this IBody2 body ,IModelDoc2 doc, System.Drawing.Color? c = null, swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
            c = c ?? Color.Yellow;
            var colorref = System.Drawing.ColorTranslator.ToWin32(c.Value);
            body.Display3(doc, colorref, (int) opt);
        }

        public static void DisplayAll(this IEnumerable<IBody2> bodies, IModelDoc2 doc, System.Drawing.Color c, swTempBodySelectOptions_e opt)
        {
            foreach (var body in bodies)
            {
                body.DisplayTs(doc, c, opt);
            }
        } 


        /// <summary>
        /// Return the bounding box of the solid body.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static TwoPointRange GetBodyBoxTs(this IBody2 body)
        {
            var box = (double[]) body.GetBodyBox();
            return new TwoPointRange(
                new[] {box[0], box[1], box[2]}, 
                new[] {box[3], box[4], box[5]});
        }
    }

    public static class DisplayTransaction
    {
        public static IDisposable DisplayUndoable(this IBody2 body, IModelDoc2 doc, System.Drawing.Color? c = null,
            swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
            c = c ?? Color.Yellow;
            body.Display3(doc, System.Drawing.ColorTranslator.ToWin32(c.Value), (int) opt);
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
