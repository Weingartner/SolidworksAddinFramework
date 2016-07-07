using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using MathNet.Numerics;
using SolidworksAddinFramework.OpenGl;
using SolidworksAddinFramework.Wpf;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{

    public class BSplineFace
    {
        public BSplineSurface Surface { get; }
        public IReadOnlyList<IReadOnlyList<BSpline>> TrimLoops { get; }

        public BSplineFace
            TransformSurfaceControlPoints(Func<Vector4[,], Vector4[,]> xformSurface, Func<Vector4[],Vector4[]> xformTrimLoops )
        {
            var surface = Surface.WithCtrlPts(xformSurface);
            var trimLoops = TrimLoops
                .Select(loop => loop.Select(curve => curve.WithControlPoints(xformTrimLoops)).ToList())
                .ToList();

            return new BSplineFace(surface, trimLoops);
        }

        internal BSplineFace(IFace2 face, double tol)
        {
            var surface = ((ISurface) face.GetSurface());
            Surface = surface.GetBSplineSurfaceParams(tol);

            TrimLoops = face
                .GetTrimLoops()
                .Select(curves => curves.Select(curve => curve.GetBSplineParams(false)).ToList())
                .ToList();
        }

        public BSplineFace(BSplineSurface surface, IReadOnlyList<IReadOnlyList<BSpline>> trimLoops)
        {
            Surface = surface;
            TrimLoops = trimLoops;
        }

        public IBody2 ToSheetBody()
        {
            var surface = Surface.ToSurface();

            var curves = TrimLoops
                .Select(loop => loop.Select(curve => curve.ToCurve()).ToList())
                .PackForTrimming();

            return (IBody2) surface.CreateTrimmedSheet4(curves, true);

        }

    }

    public static class SwFaceParamsExtensions
    {

        public static BSplineFace ToBSplineFace(this IFace2 face, double tol)
        {
            return new BSplineFace(face, tol);
        }
    }

    /// <summary>
    /// BSpline in homogeneous coordinates. Non periodic.
    /// </summary>
    public class BSpline : IEquatable<BSpline>
    {
        /// <summary>
        /// Control points are stored as (X,Y,Z) being the
        /// true location of the control point and (W) being
        /// the weight. Some frameworks store (X*W,Y*W,Z*W).
        /// This is not done here. We use the unscaled location
        /// variables. You may have to scale up and down when
        /// coverting to other systems such as eyeshot and 
        /// solidworks.
        /// 
        /// Eyeshot always stores as (X*W,Y*W,Z*W,W) in
        /// it's Point4D class. Solidworks varies depending
        /// on if the spline is periodic, closed and if 
        /// it's a tuesday or not. Grrrr. So be careful.
        /// </summary>
        public Vector4[] ControlPoints { get; }

        public bool IsPeriodic { get; }
        public bool IsClosed { get; }

        public int Order { get; }

        public double[] KnotVectorU { get; }

        public BSpline([NotNull] Vector4[] controlPoints, [NotNull] double[] knotVectorU, int order, bool isClosed)
        {

            if (controlPoints == null) throw new ArgumentNullException(nameof(controlPoints));
            if (knotVectorU == null) throw new ArgumentNullException(nameof(knotVectorU));

            ControlPoints = controlPoints;
            Order = order;
            KnotVectorU = knotVectorU;
            IsClosed = isClosed;
        }

        public BSpline WithControlPoints(Func<Vector4[], Vector4[]> transform) => 
            new BSpline(transform(ControlPoints), KnotVectorU,Order, IsClosed);

        #region equality
        public bool Equals(BSpline other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ControlPoints.SequenceEqual(other.ControlPoints)
                   && KnotVectorU.SequenceEqual(other.KnotVectorU)

                   && Order == other.Order
                   && IsClosed == other.IsClosed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BSpline) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ControlPoints.Cast<Vector4>().GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ KnotVectorU.GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ Order;
                hashCode = (hashCode*397) ^ IsClosed.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(BSpline left, BSpline right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BSpline left, BSpline right)
        {
            return !Equals(left, right);
        }
        #endregion

        public ICurve ToCurve() => ModellerExtensions.CreateBsplineCurve(ControlPoints, KnotVectorU, Order, IsPeriodic, SwAddinBase.Active.Modeler);
    }

    public static class SwBSplineParamsExtensions
    {
        public static BSpline GetBSplineParams(this ICurve swCurve, bool isClosed)
        {
            return swCurve
                .GetBCurveParams5
                    ( WantCubicIn: false
                    , WantNRational: false
                    , ForceNonPeriodic: false
                    , IsClosed: isClosed
                    )
                .SwBSplineParams(isClosed);
        }

        public static BSpline SwBSplineParams(this SplineParamData swSurfParameterisation, bool isClosed)
        {
            object ctrlPts;
            var canGetCtrlPts = swSurfParameterisation.GetControlPoints(out ctrlPts);
            Debug.Assert(canGetCtrlPts);
            var ctrlPtArray = ctrlPts.CastArray<double>();

            object knots;
            var canGetKnots = swSurfParameterisation.GetKnotPoints(out knots);
            Debug.Assert(canGetKnots);
            var knotArray = knots.CastArray<double>();

            var dimension = swSurfParameterisation.Dimension;
            var order = swSurfParameterisation.Order;
            var degree = order - 1;

            var isPeriodic = swSurfParameterisation.Periodic == 1;

            var controlPoints4D = ctrlPtArray
                .Buffer(dimension, dimension)
                .Where(p => p.Count == dimension)
                //http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.isplineparamdata~igetcontrolpoints.html
                .Select
                (p =>
                {
                    var x = 0.0;
                    var y = 0.0;
                    var z = 0.0;
                    var w = 1.0;
                    if (dimension >= 2)
                    {
                        x = p[0];
                        y = p[1];
                    }
                    else if (dimension >= 3)
                    {
                        z = p[2];
                    }
                    else if (dimension == 4)
                    {
                        w = p[3];
                    }

                    return new Vector4((float) (x*w), (float) (y*w), (float) (z*w), (float) w);
                })
                .ToArray();

            if(controlPoints4D.Any(c=>c.W!=1.0))
                LogViewer.Log($"Got a rational curve, periodic = {isPeriodic}, isClosed = {isClosed}");

            if (isPeriodic)
                ConvertToNonPeriodic(ref controlPoints4D, ref knotArray, degree);

            return new BSpline(controlPoints4D, knotArray, order, isClosed);
        }


        /// <summary>
        /// We want to convert the periodic formulation into a non periodic formulation. To do
        /// this we follow the instructions from Guilia at DevDept.
        ///
        /// the things that need to be done to get the correct curve in Eyeshot are:
        /// - add one last control point equal to the first to close the curve
        /// - multiply the(x, y, z) coordinates of each control point by its w coordinate
        /// - the degree p of your curve is given by the multiplicity of the last knot in the periodic knot vector, therefore you need to increase by 1 the multiplicity of the last knot, and by p the multiplicity of the first knot.
        /// </summary>
        /// <param name="controlPoints4D"></param>
        /// <param name="knotArray"></param>
        /// <param name="degree"></param>
        private static void ConvertToNonPeriodic(ref Vector4[] controlPoints4D, ref double[] knotArray, int degree)
        {

            controlPoints4D = controlPoints4D.EndWith(controlPoints4D.First()).ToArray();

            var last = knotArray.Last();
            var first = knotArray.First();
            knotArray = Enumerable.Repeat(first, degree).Concat(knotArray).EndWith(last).ToArray();

            // We need to do some special handling to make it non periodic
        }
    }
}