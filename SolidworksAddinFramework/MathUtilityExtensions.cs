using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using Weingartner.Numerics;

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
        public static MathPoint Origin(this IMathUtility m) => m.Point(new[] {0,0,0.0});

        public static MathTransform ComposeTransform(this IMathUtility math, MathVector translate, double scale = 1.0)
        {
            return math.ComposeTransform(math.XAxis(), math.YAxis(), math.ZAxis(), translate, scale);
        }

        public static MathTransform IdentityTransform(this IMathUtility math)
        {
            return math.ComposeTransform( math.Vector(new double[] {0,0,0}));
        }


        public static IMathPoint RotateByAngle(this IMathUtility m, IMathPoint p, IMathVector axis, double angle)
        {
            var transformation = (IMathTransform)m.CreateTransformRotateAxis(m.Origin(), axis, angle);
            return p.MultiplyTransformTs(transformation);
        }


        public static IMathPoint TranslateByVector(this IMathUtility m, IMathPoint p, IMathVector v)
        {
            var vData = v.ArrayData.CastArray<double>();
            var xForm = new[] {1, 0, 0, 0, 1, 0, 0, 0, 1, vData[0], vData[1], vData[2], 1, 0, 0, 0};
            var transformation = (IMathTransform)m.CreateTransform(xForm);
            return p.MultiplyTransformTs(transformation);
        }

        public static List<List<double>> InterpolatePoints(this IMathUtility m, IEnumerable<double[]> pointsEnum, double stepSize)
        {
            var point2Ds = pointsEnum as IList<double[]> ?? pointsEnum.ToList();

            var interpolatedPoints =  point2Ds
                .Buffer(2,1)
                .Where(p=>p.Count==2)
                .SelectMany(p =>
                {
                    var n = (int)Math.Ceiling(m.Vector(p[0],p[1]).GetLength() / stepSize);
                    n = Math.Max(2, n);
                    return Sequences.LinSpace(p[0].ToList(), p[1].ToList(), n, false);
                })
                .ToList();

            interpolatedPoints.Add(point2Ds.Last().ToList());

            interpolatedPoints = interpolatedPoints.Distinct().ToList();

            return interpolatedPoints;
        }
    }
}
