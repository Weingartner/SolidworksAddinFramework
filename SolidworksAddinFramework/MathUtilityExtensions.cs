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

        public static MathVector XAxis(this IMathUtility m) => m.Vector(new double[] {1, 0, 0});
        public static MathVector YAxis(this IMathUtility m) => m.Vector(new double[] {0, 1, 0});
        public static MathVector ZAxis(this IMathUtility m) => m.Vector(new double[] {0, 0, 1});

        public static MathTransform ComposeTransform(this IMathUtility math, MathVector translate, double scale = 1.0)
        {
            return math.ComposeTransform(math.XAxis(), math.YAxis(), math.ZAxis(), translate, scale);
        }
        public static MathTransform IdentityTransform(this IMathUtility math)
        {
            return math.ComposeTransform( math.Vector(new double[] {0,0,0}));
        }
    }
}
