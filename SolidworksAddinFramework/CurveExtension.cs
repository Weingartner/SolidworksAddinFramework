using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Accord.Math.Optimization;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using Weingartner.Numerics;

namespace SolidworksAddinFramework
{
    public struct PointParam
    {
        public readonly Vector3 Point;
        public readonly double T;

        public PointParam(Vector3 point, double t)
        {
            Point = point;
            T = t;
        }
    }

    public static class CurveExtension
    {
        /// <summary>
        /// Return the point at parameter value t on the curve.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="t"></param>
        /// <param name="derivatives"></param>
        /// <returns></returns>
        public static List<Vector3> PointAt(this ICurve curve, double t, int derivatives)
        {
            var ret = ((double[]) curve.Evaluate2(t, derivatives))
                .Buffer(3, 3)
                .Take(derivatives + 1)
                .Select(p => p.ToVector3())
                .ToList();
            return ret;
        }

        public static PointDirection3 PointTangentAt(this ICurve c, double t)
        {
            return c.PointAt(t, 1).ToPointDirection3();
        } 

        public static Vector3 PointAt(this ICurve curve, double t)
        {
            return curve.PointAt(t, 0).First();
        }


        public static PointParam ClosestPointOn(this ICurve curve , double x, double y, double z)
        {
            var array = curve
                .GetClosestPointOn(x, y, z)
                .CastArray<double>();

            var point = array.ToVector3();
            var param = array[3];
            return new PointParam(point, param);
        }

        public static PointParam ClosestPointOn(this ICurve curve, Vector3 v) => curve.ClosestPointOn(v.X, v.Y, v.Z);
        public static PointDirection3 ClosestPointTangentOn(this ICurve curve, Vector3 v) => curve.PointTangentAt(curve.ClosestPointOn(v.X, v.Y, v.Z).T);

        public static void ApplyTransform(this ICurve body, Matrix4x4 t)
        {
            var transform = SwAddinBase.Active.Math.ToSwMatrix(t);
            body.ApplyTransform(transform);
        }

        public static EdgeDistance ClosestDistanceBetweenTwoCurves(IMathUtility m,ICurve curve0, ICurve curve1)
        {
            var curveDomain = curve1.Domain();

            var solver = new BrentSearch
                (t =>
                {
                    var pt = curve1.PointAt(t);
                    return (curve0.ClosestPointOn(pt).Point - pt).Length();
                }
                , curveDomain[0]
                , curveDomain[1]
                );

            solver.Minimize();
            var param = solver.Solution;

            var pt1 = curve1.PointAt(param);
            var pt0 = curve0.ClosestPointOn(pt1).Point;
            var edge = new Edge3(pt1, pt0);
            return new EdgeDistance(edge, solver.Value);
        }

        /// <summary>
        /// Return the length of the curve between the start
        /// and end parameters.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static double Length(this ICurve curve)
        {
            bool isPeriodic;
            double end;
            bool isClosed;
            double start;
            curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);
            return curve.GetLength3(start, end);
        }


        /// <summary>
        /// Return the domain of the curve. ie the [startParam, endParam]
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static double[] Domain(this ICurve curve )
        {
            bool isPeriodic;
            double end;
            bool isClosed;
            double start;
            curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);
            return new[] {start, end};

        }

        public static List<Vector3> StartPoint(this ICurve curve, int derivatives)
        {
            var d = curve.Domain()[0];
            return curve.PointAt(d, derivatives);
        }
        public static Vector3 StartPoint(this ICurve curve)
        {
            var d = curve.Domain()[0];
            return curve.PointAt(d);
        }
        public static List<Vector3> EndPoint(this ICurve curve, int derivatives )
        {
            var d = curve.Domain()[1];
            return curve.PointAt(d, derivatives);
        }
        public static Vector3 EndPoint(this ICurve curve)
        {
            var d = curve.Domain()[1];
            return curve.PointAt(d);
        }

        public static Vector3[] GetTessPoints(this ICurve curve, double chordTol, double lengthTol)
        {
            bool isPeriodic;
            double end;
            bool isClosed;
            double start;
            curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);
            var startPt = (double[]) curve.Evaluate2(start, 0);
            var midPt = (double[]) curve.Evaluate2((start + end)/2, 0);
            var endPt = (double[]) curve.Evaluate2(end, 0);
            var set0 = GetTessPotsAsVector3(curve, chordTol, lengthTol, startPt, midPt)
                .ToArray();

            var set1 = GetTessPotsAsVector3(curve, chordTol, lengthTol, midPt, endPt)
                .ToArray();


            return set0.Concat(set1).ToArray();
        }

        private static IEnumerable<Vector3> GetTessPotsAsVector3(ICurve curve, double chordTol, double lengthTol, double[] startPt, double[] midPt)
        {
            return ((double[]) curve.GetTessPts(chordTol, lengthTol, startPt, midPt))
                .Buffer(3, 3)
                .Select(b => new Vector3((float) b[0], (float) b[1], (float) b[2]));
        }

        public static ICurve GetCurveTs(this IEdge edge)
        {
            return (ICurve)edge.GetCurve();
        }

        public static List<Vector3> GetPointsByLength(this ICurve curve, double pointDistance)
        {
            var length = curve.Length();
            var numberOfPoints = (int)(length/pointDistance);

            var domain = curve.Domain();
            return Sequences.LinSpace(domain[0], domain[1], numberOfPoints)
                .Select(t => curve.PointAt(t))
                .ToList();
        }
    }
}
