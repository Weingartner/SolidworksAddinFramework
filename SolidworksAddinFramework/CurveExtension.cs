using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math.Optimization;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class CurveExtension
    {
        /// <summary>
        /// Return the point at parameter value t on the curve.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double[] PointAt(this ICurve curve, double t, int derivatives = 0)
        {
            var ret = (double[]) curve.Evaluate2(t, derivatives);
            //Evaluate2 returns always an additional value x,y,z, value which doesn't make sense
            //Therefore we Skip the last value
            ret = ret.SkipLast(1).ToArray();
            return ret;
        }

        public static double[] ClosestPointOnTs(this ICurve curve , double x, double y, double z)
        {
            return (double[]) curve.GetClosestPointOn(x, y, z);
        }

        public static Tuple<double[], double[], double> ClosestDistanceBetweenTwoCurves(IMathUtility m,ICurve curve0, ICurve curve1)
        {
            var curveDomain = curve1.Domain();

            var solver = new BrentSearch
                (t =>
                {
                    var pt = curve1.PointAt(t);
                    var closestPoint = curve0.ClosestPointOnTs(pt[0], pt[1], pt[2]).Take(3).ToArray();
                    var distance = m.Vector(pt, closestPoint).GetLength();
                    return distance;
                }
                , curveDomain[0]
                , curveDomain[1]
                );

            solver.Minimize();
            var param = solver.Solution;

            var pt1 = curve1.PointAt(param);
            var pt0 = curve0.ClosestPointOnTs(pt1[0],pt1[1],pt1[2]).Take(3).ToArray();
            return Tuple.Create(pt0, pt1, solver.Value);
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
        public static double[] Domain(this ICurve	 curve )
        {
            bool isPeriodic;
            double end;
            bool isClosed;
            double start;
            curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);
            return new[] {start, end};

        }

        public static double[] StartPoint(this ICurve curve, int derivatives = 0)
        {
            var d = curve.Domain()[0];
            return curve.PointAt(d, derivatives);
        }
        public static double[] EndPoint(this ICurve curve, int derivatives = 0)
        {
            var d = curve.Domain()[1];
            return curve.PointAt(d, derivatives);
        }

        public static double[][] GetTessPoints(this ICurve curve, double chordTol, double lengthTol)
        {
            bool isPeriodic;
            double end;
            bool isClosed;
            double start;
            curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);
            var startPt = (double[]) curve.Evaluate2(start, 0);
            var midPt = (double[]) curve.Evaluate2((start + end)/2, 0);
            var endPt = (double[]) curve.Evaluate2(end, 0);
            var set0 = ((double[]) curve.GetTessPts(chordTol, lengthTol, startPt, midPt))
                .Buffer(3, 3)
                .Select(b => b.ToArray())
                .ToArray();
            ;

            var set1 = ((double[]) curve.GetTessPts(chordTol, lengthTol, midPt, endPt))
                .Buffer(3, 3)
                .Select(b => b.ToArray())
                .ToArray();
            ;


            return set0.Concat(set1).ToArray();
        }

        public static ICurve GetCurveTs(this IEdge edge)
        {
            return (ICurve)edge.GetCurve();
        }

        /// <summary>
        /// Finds the closest point on a curve to a given zPosition.
        /// Assumes a continous curve in z, otherwise it might return a local minimum!
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="zPos"></param>
        /// <returns></returns>
        public static double[] ClosestPointToZPosition(this ICurve curve, double zPos, out double par)
        {
            var curveDomain = curve.Domain();
            Func<double, double> fn = (t) =>
            {
                var ptZ = curve.PointAt(t)[2];
                return (ptZ - zPos);
            };

            try
            {
                var solver = new BrentSearch(fn, curveDomain[0], curveDomain[1]);
                solver.FindRoot();
                par = solver.Solution;
            }
            catch (Exception)
            {
                throw new Exception("Solver solution doesn't converge!");
            }
            return curve.PointAt(par).Take(3).ToArray();
        }
    }
}
