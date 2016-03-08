using System;
using System.Collections.Generic;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class MathUtilityExtensions
    {
        public static MathVector Vector(this IMathUtility m, double[] v)
        {
            return (MathVector)m.CreateVector(v);
        }
        public static MathVector Vector(this IMathUtility m, double[] a, double [] b)
        {
            var p0 = m.Point(a);
            var p1 = m.Point(b);
            return p1.SubtractTs(p0);
        }
        public static MathPoint Point(this IMathUtility m, double[] v)
        {
            return (MathPoint)m.CreatePoint(v);
        }
    }
}
