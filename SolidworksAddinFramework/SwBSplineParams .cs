using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public class SwFaceParams
    {
        public SwBSplineSurfaceParams Surface { get; }
        public IReadOnlyList<IReadOnlyList<SwBSplineParams>> TrimLoops { get; }

        public SwFaceParams
            TransformSurfaceControlPoints(Func<Vector4[,], Vector4[,]> xformSurface, Func<Vector4[],Vector4[]> xformTrimLoops )
        {
            var surface = Surface.WithCtrlPts(xformSurface);
            var trimLoops = TrimLoops
                .Select(loop => loop.Select(curve => curve.WithControlPoints(xformTrimLoops)).ToList())
                .ToList();

            return new SwFaceParams(surface, trimLoops);
        }

        public SwFaceParams(IFace2 face, double tol)
        {
            var surface = ((ISurface) face.GetSurface());
            Surface = surface.GetBSplineSurfaceParams(tol);

            TrimLoops = face
                .GetTrimLoops()
                .Select(curves => curves.Select(curve => curve.GetBSplineParams(false, tol)).ToList())
                .ToList();
        }

        private SwFaceParams(SwBSplineSurfaceParams surface, IReadOnlyList<IReadOnlyList<SwBSplineParams>> trimLoops)
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

    public class SwBSplineParams : IEquatable<SwBSplineParams>
    {
        public Vector4[] ControlPoints { get; }
        
        public int ControlPointDimension { get; }
        public bool IsPeriodic { get; }

        public int SwOrderU { get; }

        public double[] KnotVectorU { get; }

        public SwBSplineParams([NotNull] Vector4[] controlPoints, int swOrderU, [NotNull] double[] knotVectorU, int controlPointDimension, bool isPeriodic)
        {

            if (controlPoints == null) throw new ArgumentNullException(nameof(controlPoints));
            if (knotVectorU == null) throw new ArgumentNullException(nameof(knotVectorU));

            ControlPoints = controlPoints;
            SwOrderU = swOrderU;
            KnotVectorU = knotVectorU;
            ControlPointDimension = controlPointDimension;
            IsPeriodic = isPeriodic;
        }

        public SwBSplineParams WithControlPoints(Func<Vector4[], Vector4[]> transform) => new SwBSplineParams(transform(ControlPoints),SwOrderU, KnotVectorU,ControlPointDimension,IsPeriodic);

        #region equality
        public bool Equals(SwBSplineParams other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ControlPoints.SequenceEqual(other.ControlPoints)
                   && KnotVectorU.SequenceEqual(other.KnotVectorU)

                   && SwOrderU == other.SwOrderU;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SwBSplineParams) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ControlPoints.Cast<Vector4>().GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ KnotVectorU.GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ SwOrderU;
                return hashCode;
            }
        }

        public static bool operator ==(SwBSplineParams left, SwBSplineParams right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SwBSplineParams left, SwBSplineParams right)
        {
            return !Equals(left, right);
        }
        #endregion

        public ICurve ToCurve()
        {
            var modeler = SwAddinBase.Active.Modeler;
            var controlPointsList = ControlPoints
                .SelectMany(p=> new double[] {p.X, p.Y, p.Z, p.W}.Take(ControlPointDimension).ToArray())
                .ToArray();

            var dimensionControlPoints = BitConverter.GetBytes(ControlPointDimension);
            var order = BitConverter.GetBytes((int) SwOrderU);
            var numControlPoints = BitConverter.GetBytes((int) ControlPoints.Length);

            var periodicity = BitConverter.GetBytes(IsPeriodic? 1 : 0);

            var props = new[]
            {
                BitConverter.ToDouble(dimensionControlPoints.Concat(order).ToArray(), 0),
                BitConverter.ToDouble(numControlPoints.Concat(periodicity).ToArray(), 0)
            };

            var swCurve = (Curve) modeler.CreateBsplineCurve(props, KnotVectorU, controlPointsList);
            var domain = swCurve.Domain();
            var eps = Math.Abs(domain[0] - domain[1])/1e6;
            return swCurve;
            //if(Math.Abs(domain[0] - T0) < eps && Math.Abs(domain[1] - T1) < eps)
            //    return swCurve;
            //return swCurve.CreateTrimmedCurve(T0, T1);
        }
    }

    public static class SwBSplineParamsExtensions
    {
        public static SwBSplineParams GetBSplineParams(this ICurve swCurve, bool isClosed, double tol)
        {

            var swSurfParameterisation = swCurve.GetBCurveParams5(false, false, false, isClosed);

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

            var isPeriodic = swSurfParameterisation.Periodic == 1;

            var controlPoints4D = ctrlPtArray
                .Buffer(dimension, dimension)
                .Where(p=>p.Count==dimension)
                //http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.isplineparamdata~igetcontrolpoints.html
                .Select(p =>
                {
                    double x=0.0;
                    double y=0.0;
                    double z=0.0;
                    double w=0.0; 
                    if (dimension == 2)
                    {
                        x = p[0];
                        y = p[1];
                    }else if (dimension == 3)
                    {
                        x = p[0];
                        y = p[1];
                        z = p[2];
                        
                    }else if (dimension == 4)
                    {
                        x = p[0];
                        y = p[1];
                        z = p[2];
                        w = p[3];
                    }
                    if (isPeriodic)
                    {
                        x /= w;
                        y /= w;
                        z /= w;
                    }
                    return new Vector4((float) x,(float) y,(float) z,(float) w);

                })
                .ToArray();

            double start;
            double end;
            swCurve.GetEndParams(out start, out end, out isClosed, out isPeriodic);
            return new SwBSplineParams(controlPoints4D, order, knotArray, dimension, isPeriodic);
        }
    }
}