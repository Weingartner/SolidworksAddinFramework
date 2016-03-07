using System;
using System.Collections.Generic;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class MathPointExtensions
    {
        public static IMathPoint Project(this IMathPoint point, IMathPoint origin, IMathVector axis)
        {
            var a = (IMathVector) point.Subtract(origin);
            var t = a.Project(axis);
            var v = (MathVector) axis.Scale(t);
            return (IMathPoint) origin.AddVector(v);
        }
    }

    public static class MathVectorExtensions
    {
        /// <summary>
        /// gives the multiplier for b which would be the projection of a on b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Project(this IMathVector a, IMathVector b)
        {
            return a.Dot(b)/(b.Dot(b));
        }
    }
}
