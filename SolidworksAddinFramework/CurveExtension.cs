using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using Accord.Math.Optimization;
using AForge;
using MathNet.Numerics;
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

    public struct PointParamWithRayProjection
    {
        public readonly PointParam PointParam;
        public readonly Vector3 RayPoint;

        public PointParamWithRayProjection(PointParam pointParam, Vector3 rayPoint)
        {
            PointParam = pointParam;
            RayPoint = rayPoint;
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

        /// <summary>
        /// Should solve
        /// </summary>
        /// <param name="c"></param>
        /// <param name="t0"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static double PointAtDistanceFrom(this ICurve c, double t0, double distance)
        {
            Func<double, double> objFunc = t1 =>
            {
                var length3 = t0 < t1 ? c.GetLength3(t0, t1) : c.GetLength3(t1, t0);
                return length3 - Math.Abs(distance);
            };
            var domain = c.Domain();
            var min = distance < 0.0 ? 0.8*domain[0] : t0;
            var max = distance < 0.0 ? t0 : 1.2*domain[1];
            var solver = new BrentSearch(objFunc, min, max);
            solver.FindRoot();
            var sol = solver.Solution;
            return sol;
        }
        public static double PointAtDistanceFrom(this ICurve c, Vector3 p0, double distance)
        {
            var ptT0 = c.ClosestPointOn(p0);
            return PointAtDistanceFrom(c, ptT0.T, distance);
        }

        public static IDisposable DisplayUndoable(
            this ICurve curve
            , IModelDoc2 doc
            , Color color
            , double thickness
            , double chordTol=1e-6
            , double lengthTol=0
            , int layer = 0)
        {
            var wire = new OpenWire(curve,thickness, color, chordTol, lengthTol);
            return wire.DisplayUndoable(doc, layer);
        }

        public static IDisposable DisplayUndoable
            (
            this Edge3 line
            ,
            IModelDoc2 doc
            ,
            Color color
            ,
            float thickness = 1
            ,
            int layer = 0
            )
        {
            var wire = new OpenWire(new [] {line.A, line.B}.ToList(),thickness, color);
            return wire.DisplayUndoable(doc, layer);
        }

        public static PointDirection3 PointTangentAt(this ICurve c, double t)
        {
            return c.PointAt(t, 1).ToPointDirection3();
        } 

        public static Vector3 PointAt(this ICurve curve, double t)
        {
            return curve.PointAt(t, 0).First();
        }

        public static PointParam PointParamAt(this ICurve curve, double t)=>new PointParam(curve.PointAt(t), t);


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

        public static Vector3[] GetTessPoints(this ICurve curve, double chordTol, double lengthTol, RangeDouble? domain = null)
        {
            bool isPeriodic;
            double end;
            bool isClosed;
            double start;
            if (domain == null)
                curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);
            else
            {
                start = domain.Value.Min;
                end = domain.Value.Max;
            }

            var startPt = (double[]) curve.Evaluate2(start, 0);
            var midPt = (double[]) curve.Evaluate2((start + end)/2, 0);
            var endPt = (double[]) curve.Evaluate2(end, 0);
            var set0 = GetTessPotsAsVector3(curve, chordTol, lengthTol, startPt, midPt)
                .ToArray();

            var set1 = GetTessPotsAsVector3(curve, chordTol, lengthTol, midPt, endPt)
                .ToArray();


            return set0.Concat(set1.Skip(1)).ToArray();
        }

        public class MinimumRadiusResult
        {
            /// <summary>
            /// Location on the curve of the point of minimum radius
            /// </summary>
            public Vector3 Location { get; }
            /// <summary>
            /// The value of the minimum radius
            /// </summary>
            public double Radius { get; }
            /// <summary>
            /// The value of the minimum radius
            /// </summary>
            public double T { get; }

            public MinimumRadiusResult(double t, double radius, Vector3 location)
            {
                this.T = t;
                this.Radius = radius;
                this.Location = location;
            }
        }

        public static MinimumRadiusResult FindMinimumRadius(this ICurve curve)
        {
            var bound = curve.Domain();

            int numOfRadius = 0;
            object radiusObject = null;
            object locationObject = null;
            object tObject = null;
            var hasRadius = curve.FindMinimumRadius(bound
                , ref numOfRadius
                , ref radiusObject
                , ref locationObject
                , ref tObject);

            if (hasRadius && numOfRadius > 0)
            {

                return new MinimumRadiusResult
                    (t:((double[])tObject)[0]
                    ,radius:((double[])radiusObject)[0]
                    ,location:((MathPoint)((object[])locationObject)[0]).ToVector3());
            }
            else
            {
                return null;
            }

        }

        public static double ReverseEvaluate(this ICurve curve, Vector3 p) => curve.ReverseEvaluate(p.X, p.Y, p.Z);

        public static PointParamWithRayProjection ClosestPointToRay(this ICurve curve, PointDirection3 ray, double tol = 1e-9)
        {
            var bound = curve.Domain();
            int numOfRadius = 0;
            double[] radius = null;
            MathPoint location = null;

            var radiusResult = curve.FindMinimumRadius();
            var domain = new RangeDouble(curve.Domain());
            var tessTol = radiusResult.Radius/10;
            var closestPointOnEdge = Vector3.Zero;
            for (var i = 0; i < 1; i++)
            {
                var tessPoints = Sequences
                    .LinSpace(domain.Min, domain.Max, 100)
                    .Select(curve.PointParamAt).ToList();
                var edges = tessPoints.Buffer(2, 1).Where(buf => buf.Count == 2)
                    .ToList();

                var closestEdge
                    = edges
                        .Select(edge => new {edge, connection= MakeEdge(edge).ShortestEdgeJoining(ray, tol)})
                        .MinBy(o=>o.connection.LengthSquared)[0];

                var a = closestEdge.edge[0].T;
                var b = closestEdge.edge[1].T;
                domain = new RangeDouble(a, b);
                tessTol = tessTol/10;
            }

            Func<Vector3, Vector3> projectOnRay = p => (p - ray.Point).ProjectOn(ray.Direction) + ray.Point;

            var solver = new BrentSearch(t =>
            {
                var p = curve.PointAt(t);
                var proj = projectOnRay(p);
                return (p - proj).LengthSquared();
            }, domain.Min, domain.Max);
            solver.Minimize();
            var minT = solver.Solution;

            var pointParam = curve.PointParamAt(minT);
            return new PointParamWithRayProjection(pointParam, projectOnRay(pointParam.Point));
        }

        private static Edge3 MakeEdge(IList<PointParam> edge)
        {
            return new Edge3(edge[0].Point, edge[1].Point);
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

        /// <summary>
        /// Approximation. Be carefull
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="pointDistance"></param>
        /// <param name="flankHelixDomain"></param>
        /// <returns></returns>
        public static List<PointParam> GetPointsByLength
            ( this ICurve curve
            , double pointDistance
            , DoubleRange? flankHelixDomain = null)
        {
            var length = curve.Length();
            var numberOfPoints = (int)(length/pointDistance);

            var curveDomain = curve.Domain();
            var domain = flankHelixDomain ?? new DoubleRange(curveDomain[0], curveDomain[1]);

            return Sequences.LinSpace(domain.Min, domain.Max, numberOfPoints)
                .Select(t => new PointParam(curve.PointAt(t), t))
                .ToList();
        }

        public static void GetPlanePoints(this ICurve curve, double position, out Vector3 pNormal, out Vector3 pCurve, out Vector3 pAxis)
        {
            var pCurvePoint = curve.PointTangentAt(position);

            pCurve = pCurvePoint.Point;
            pAxis = pCurve.ProjectOn(Vector3.UnitZ);

            var vTangent = pCurvePoint.Direction;
            var vCa = pAxis - pCurve;

            var vNormal = Vector3.Cross(vTangent, vCa);

            pNormal = pCurve + vNormal;
        }
    }
}
