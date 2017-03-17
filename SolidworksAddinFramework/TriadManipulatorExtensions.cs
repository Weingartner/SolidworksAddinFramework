using System;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class TriadManipulatorExtensions
    {
        public static MathTransform CreateTranslationTransform
            (this ITriadManipulator m, swTriadManipulatorControlPoints_e h, IMathUtility math, double value)
        {
            MathVector translVector = null;
            switch (h)
            {
                case swTriadManipulatorControlPoints_e.swTriadManipulatorOrigin:
                    break;
                case swTriadManipulatorControlPoints_e.swTriadManipulatorXAxis:
                    translVector = (MathVector)math.CreateVector(new[] {value, 0, 0});
                    break;
                case swTriadManipulatorControlPoints_e.swTriadManipulatorYAxis:
                    translVector = (MathVector)math.CreateVector(new[] {0, value, 0});
                    break;
                case swTriadManipulatorControlPoints_e.swTriadManipulatorZAxis:
                    translVector = (MathVector)math.CreateVector(new[] {0, 0, value});
                    break;
                case swTriadManipulatorControlPoints_e.swTriadManipulatorXYPlane:
                    break;
                case swTriadManipulatorControlPoints_e.swTriadManipulatorYZPlane:
                    break;
                case swTriadManipulatorControlPoints_e.swTriadManipulatorZXPlane:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(h), h, null);
            }

            if (translVector == null)
                return null;

            return math.ComposeTransform(m.XAxis, m.YAxis, m.ZAxis, translVector, 1.0);
        }

        public static MathVector Project(this ITriadManipulator m, swTriadManipulatorControlPoints_e h, IMathPoint p, IMathVector cameraVector, IMathUtility mathP)
        {
            IMathUtility math = mathP;
            var zero = (IMathPoint) math.CreatePoint(new[] {0, 0, 0});
            double pT, qT;
            switch (h)
            {
                case swTriadManipulatorControlPoints_e.swTriadManipulatorOrigin:
                    return (MathVector) p.Subtract(m.Origin);
                case swTriadManipulatorControlPoints_e.swTriadManipulatorXAxis:

                    return ClosestPointOnAxis(m, p, cameraVector, m.XAxis);
                case swTriadManipulatorControlPoints_e.swTriadManipulatorYAxis:
                    return ClosestPointOnAxis(m, p, cameraVector, m.YAxis);
                case swTriadManipulatorControlPoints_e.swTriadManipulatorZAxis:
                    return ClosestPointOnAxis(m, p, cameraVector, m.ZAxis);
                case swTriadManipulatorControlPoints_e.swTriadManipulatorXYPlane:
                    return (MathVector) p.Subtract(m.Origin);
                case swTriadManipulatorControlPoints_e.swTriadManipulatorYZPlane:
                    return (MathVector) p.Subtract(m.Origin);
                case swTriadManipulatorControlPoints_e.swTriadManipulatorZXPlane:
                    return (MathVector) p.Subtract(m.Origin);
                default:
                    throw new ArgumentOutOfRangeException(nameof(h), h, null);
            }
        }

        private static MathVector ClosestPointOnAxis(ITriadManipulator m, IMathPoint p, IMathVector cameraVector, MathVector axis)
        {
            double pT;
            double qT;

            if (ClosestPointBetweenLines(m.Origin, axis, p, cameraVector, out pT, out qT))
            {
                return (MathVector) axis.Scale(pT);
            }
            else
            {
                return null;
            }
        }

        private static MathVector ProjectRelative(IMathPoint origin, IMathVector axis, IMathPoint point)
        {
            return (MathVector) point.Project(origin, axis).Subtract(origin);
        }

        /// <summary>
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="pa"></param>
        /// <param name="va"></param>
        /// <param name="pb"></param>
        /// <param name="vb"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool ClosestPointBetweenLines(IMathPoint pOrigin, IMathVector pVector, IMathPoint qOrigin, IMathVector qVector, out double pT, out double qT)
        {
            var w0 = (IMathVector) (pOrigin.Subtract(qOrigin));
            var u = pVector.Normalise();
            var v = qVector.Normalise();
            var a = u.Dot(u);
            var b = u.Dot(v);
            var c = v.Dot(v);
            var d = u.Dot(w0);
            var e = v.Dot(w0);

            var den = (a*c - b*b);
            if (Math.Abs(den) < 1e-12)
            {
                pT = 0;
                qT = 0;
                return false;
            }
            pT = (b*e - c*d)/den;
            qT = (a*a - b*d)/den;
            return true;
        }

        private static void Destructure(MathPoint p, out double x, out double y, out double z)
        {
            var arrayData = (double[]) p.ArrayData;
            x = arrayData[0];
            y = arrayData[1];
            z = arrayData[2];
        }

        private static double LinePlaneIntersection(MathPoint origin, MathVector axis0, MathVector axis1, MathPoint lineOrigin, MathVector lineVector)
        {
            double x1, y1, z1;
            double x2, y2, z2;
            double x3, y3, z3;
            double x4, y4, z4;
            double x5, y5, z5;
            double t;

            var p0 = (MathPoint) origin.AddVector(axis0);
            var p1 = (MathPoint) origin.AddVector(axis1);

            Destructure(origin, out x1, out y1, out z1);
            Destructure(p0, out x2, out y2, out z2);
            Destructure(p1, out x3, out y3, out z3);

            var p2 = (MathPoint) origin.AddVector(lineVector);
            Destructure(lineOrigin, out x4, out y4, out z4);
            Destructure(p2, out x5, out y5, out z5);

            Matrix a = new DenseMatrix(4, 4, new double[]
            {
                1, x1, y1, z1, 1, x2, y2, z2, 1, x3, y3, z3, 1, x4, y4, z4
            });

            Matrix b = new DenseMatrix(4, 4, new double[]
            {
                1, x1, y1, z1, 1, x2, y2, z2, 1, x3, y3, z3, 0, x5 - x4, y5 - y4, z5 - z4
            });

            var da = a.Determinant();
            var db = b.Determinant();
            t = da/db;

            return t;
        }
    }
}
