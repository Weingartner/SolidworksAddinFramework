using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class ModellerExtensions
    {
        /// <summary>
        /// Create a line from p0 to p1. 
        /// </summary>
        /// <param name="modeler"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static ICurve CreateTrimmedLine(this IModeler modeler, Edge3 edge)
        {
            Debug.Assert(edge.Delta.Length() > float.Epsilon);
            var startPoint = new[] {(double)edge.A.X, (double)edge.A.Y,(double)edge.A.Z};
            var dir = edge.Delta.Unit();
            var direction = new[] {(double)dir.X,(double)dir.Y,(double)dir.Z};
            var line = (ICurve)modeler.CreateLine(startPoint, direction);
            Debug.Assert(line != null, "line != null");
            var pp0 = line.GetClosestPointOn(edge.A.X, edge.A.Y, edge.A.Z).CastArray<double>();
            var pp1 = line.GetClosestPointOn(edge.B.X, edge.B.Y, edge.B.Z).CastArray<double>();
            //line = line.CreateTrimmedCurve2(pp0[0], pp0[1], pp0[2], pp1[0], pp1[1], pp1[2]);
            line = line.ICreateTrimmedCurve(pp0[3], pp1[3]);
            Debug.Assert(line != null, "line != null");
            return line;
        }

        public static ICurve CreateTrimmedLine(this IModeler modeler, Vector3 p0, Vector3  p1)
        {
            return CreateTrimmedLine(modeler, new Edge3(p0, p1));
        }

        public static IBody2 CreateBodyFromCylTs(this IModeler modeler, Vector3  xyz, Vector3 axis, double radius, double length)
        {
            var array = xyz
                .ToDoubles()
                .Concat(axis.ToDoubles())
                .Concat(new[] {radius, length})
                .ToArray();

            return (IBody2)modeler.CreateBodyFromCyl(array);
        }


        public static ICurve CreateTrimmedLine(this IModeler modeler, MathPoint p0, MathPoint p1)
        {
            return CreateTrimmedLine( modeler, p0.ArrayData.CastArray<double>().ToVector3(), p1.ArrayData.CastArray<double>().ToVector3());
        }
        public static ICurve CreateTrimmedLine(this IModeler modeler, Vector3 p0, Vector3 v0, double length)
        {
            v0 = v0.Unit() * (float) length;
            var p1 = p0 + v0;
            return CreateTrimmedLine(modeler,p0, p1);
        }


        #region CreateSheetFromSurface
        public static IBody2 CreateSheetFromSurface(this IModeler modeler, ISurface surf , SurfaceExtensions.PointUv uvLow, SurfaceExtensions.PointUv uvHigh)
        {
            var uRange = new double[] {uvLow.U, uvHigh.U}.OrderBy(v=>v).ToArray();
            var vRange = new double[] {uvLow.V, uvHigh.V}.OrderBy(v=>v).ToArray();
            return CreateSheetFromSurface(modeler, surf, uRange, vRange);
        }

        private static IBody2 CreateSheetFromSurface(IModeler modeler, ISurface surf, double[] uRange, double[] vRange)
        {
            var uvRange = uRange.Concat(vRange).ToArray();
            var sheet = (IBody2) modeler.CreateSheetFromSurface(surf, uvRange);
            return sheet;
        }

        #endregion

        /// <summary>
        /// Create a bounded sheet.
        /// </summary>
        /// <param name="modeler"></param>
        /// <param name="center">Center of the surface</param>
        /// <param name="vNormal">Direction of the surface</param>
        /// <param name="p0">Point to project onto surface to find UV bounds</param>
        /// <param name="p1">Point to project onto surface to find UV bounds</param>
        /// <returns></returns>
        public static IBody2 CreateSheet(this IModeler modeler, Vector3 center, Vector3 vNormal, float r)
        {
            var surf = (Surface) modeler.CreatePlanarSurface(center.ToDoubles(), vNormal.ToDoubles());
            var mid = surf.GetClosestPointOnTs(center);
            var midPoint = mid.Point.ToVector3();
            var upVector = (surf.PointAt(mid.U, mid.V + 1) - midPoint).Unit();
            var rightVector = (surf.PointAt(mid.U + 1, mid.V) - midPoint).Unit();


            var low = midPoint - upVector*r - rightVector*r;
            var high = midPoint + upVector*r + rightVector*r;

            var uvLow = surf.GetClosestPointOnTs(low);
            var uvHigh = surf.GetClosestPointOnTs(high);
            return modeler.CreateSheetFromSurface(surf, uvLow, uvHigh);
        }

        public static IBody2 CreateSheetFromSurface(this IModeler modeler, ISurface surf, IMathPoint p0, IMathPoint p1)
        {
            var uvLow = surf.GetClosestPointOnTs(p0.ArrayData.CastArray<double>().ToVector3());
            var uvHigh = surf.GetClosestPointOnTs(p1.ArrayData.CastArray<double>().ToVector3());
            return modeler.CreateSheetFromSurface(surf, uvLow, uvHigh);
        }

        public static Curve InterpolatePointsToCurve
            ( this IModeler modeler
            , List<Vector3> points
            , double chordTolerance
            , bool simplify = true
            , bool closedCurve = false)
        {
            points = FilterOutShortLines(points, 1e-5).ToList();

            if (closedCurve)
            {
                points.Add(points.First());
            }

            var lines = points
                .Buffer(2, 1)
                .Where(b => b.Count == 2)
                .Select(ps => modeler.CreateTrimmedLine(ps[0], ps[1]))
                .Cast<ICurve>()
                .ToArray();

            var curve = modeler.MergeCurves(lines);
            if(simplify)
                curve = curve.SimplifyBCurve(chordTolerance);
            return curve;
        }

        public static IEnumerable<Vector3 > FilterOutShortLines(List<Vector3 > points, double tol)
        {
            Vector3?  previous = null;
            Func<Vector3 , Vector3 , double> distance = (p0,p1)=>(p0-p1).Length();
            var result = new List<Vector3 >();
            foreach (var pt in points)
            {
                if (previous == null || distance(pt, previous.Value) > tol)
                {
                    result.Add(pt);
                    previous = pt;
                }
                
            }
            result.Add(points.Last());

            result.Reverse();
            points = result;
            result = new List<Vector3 >();
            previous = null;
            foreach (var pt in points)
            {
                if (previous == null || distance(pt, previous.Value) > tol)
                {
                    result.Add(pt);
                    previous = pt;
                }
                
            }

            result.Reverse();

            return result;
        }

        public static IBody2 CreateSurfaceBody(this IModeler modeler, ISurface surface)
        {
            var swSurfPara = surface.Parameterization2();

            var uvrange = new double[]
            {
                swSurfPara.UMin,
                swSurfPara.UMax,
                swSurfPara.VMin,
                swSurfPara.VMax
            };

            return (IBody2) modeler.CreateSheetFromSurface(surface, uvrange);
        }

        public static IBody2 CreateSphereBody(this IModeler modeler, Vector3 center, double radius)
        {
            var axis = Vector3.UnitZ;
            var refaxis = Vector3.UnitY;
            var sphere = (ISurface)modeler.CreateSphericalSurface2(center.ToDoubles(), axis.ToDoubles(), refaxis.ToDoubles(), radius);

            var swSurfPara = sphere.Parameterization2();

            var uvrange = new double[]
            {
                swSurfPara.UMin,
                swSurfPara.UMax,
                swSurfPara.VMin,
                swSurfPara.VMax
            };


            var sphereBody = (IBody2) modeler.CreateSheetFromSurface(sphere, uvrange);
            return sphereBody;
        }

        public static IBody2 CreateBox
            (this IModeler modeler
            , Vector3 center 
            , Vector3 axis
            , double width
            , double length
            , double height
            )
        {
            var array = new List<double>();
            array.AddRange(center.ToDoubles());
            array.AddRange(axis.ToDoubles());
            array.Add(width);
            array.Add(length);
            array.Add(height);
            return modeler.CreateBodyFromBox3(array.ToArray());
        }

        public static IBody2 CreateBox(this IModeler modeler,
            double width,
            double length,
            double height) => modeler.CreateBox
                ( Vector3.Zero,
                    Vector3.UnitZ,
                    width,
                    length,
                    height);
    }
}
