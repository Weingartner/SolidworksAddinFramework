using System;
using System.Collections.Generic;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class TriadManipulatorExtensions
    {
        public static MathVector Project(this ITriadManipulator m, swTriadManipulatorControlPoints_e h, IMathPoint p, IMathVector cameraVector, IMathUtility mathP)
        {
            IMathUtility math = mathP;
            var zero = (IMathPoint)math.CreatePoint(new[] {0, 0, 0});
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

        private static MathVector ClosestPointOnAxis(ITriadManipulator m, IMathPoint p, IMathVector cameraVector,
            MathVector axis)
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

        /// <summary>
        /// gives the multiplier for b which would be the projection of a on b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static double ProjectAOnB(IMathVector a, IMathVector b)
        {
            return a.Dot(b)/(b.Dot(b));
        }

        private static IMathPoint Project(IMathPoint origin, IMathVector axis, IMathPoint point)
        {
            var a = (IMathVector) point.Subtract(origin);
            var t = ProjectAOnB(a, axis);
            var v = (MathVector) axis.Scale(t);
            return (IMathPoint)origin.AddVector(v);
        }

        private static MathVector ProjectRelative(IMathPoint origin, IMathVector axis, IMathPoint point)
        {
            return (MathVector) Project(origin, axis, point).Subtract(origin);
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
        private static bool ClosestPointBetweenLines 
            (IMathPoint pOrigin, IMathVector pVector, IMathPoint qOrigin, IMathVector qVector, out double pT, out double qT)
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
    }
}
