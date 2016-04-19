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


        private static MathVector _XAxis = null;
        private static MathVector _YAxis = null;
        private static MathVector _ZAxis = null;
        private static MathPoint _Origin = null;



        private static readonly double[] _XAxisArray = {1, 0, 0};
        private static readonly double[] _YAxisArray = {0, 1, 0};
        private static readonly double[] _ZAxisArray = {0, 0, 1};
        private static readonly double[] _OriginArray = {0, 0, 0};

        public static double [] XAxisArray(this IMathUtility m) => _XAxisArray;
        public static double [] YAxisArray(this IMathUtility m) => _YAxisArray;
        public static double [] ZAxisArray(this IMathUtility m) => _ZAxisArray;
        public static double [] OriginArray(this IMathUtility m) => _OriginArray;


        public static MathVector XAxis(this IMathUtility m) => _XAxis ?? (_XAxis = m.Vector(_XAxisArray));
        public static MathVector YAxis(this IMathUtility m) => _YAxis ?? (_YAxis = m.Vector(_YAxisArray));
        public static MathVector ZAxis(this IMathUtility m) => _ZAxis ?? (_ZAxis = m.Vector(_ZAxisArray));
        public static MathPoint Origin(this IMathUtility m) => _Origin ??(_Origin =  m.Point(_OriginArray));

        public static MathVector Mv(this IMathUtility m, double[] v) => m.Vector(v);
        public static MathPoint Mp(this IMathUtility m , double[] v) => m.Point(v);

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
            var transformation = GetRotationFromAxisAndAngle(m, axis, angle);
            return p.MultiplyTransformTs(transformation);
        }

        public static MathTransform GetRotationFromAxisAndAngle(this IMathUtility m, IMathVector axis, double angle)
        {
            return (MathTransform) m.CreateTransformRotateAxis(m.Origin(), axis, angle);
        }


        public static IMathPoint TranslateByVector(this IMathUtility m, IMathPoint p, IMathVector v)
        {
            var transformation = GetTranslationFromVector(m, v);
            return p.MultiplyTransformTs(transformation);
        }

        public static MathTransform GetTranslationFromVector(this IMathUtility m, IMathVector v)
        {
            var vData = v.ArrayData.CastArray<double>();
            var xForm = new[] {1, 0, 0, 0, 1, 0, 0, 0, 1, vData[0], vData[1], vData[2], 1, 0, 0, 0};
            return (MathTransform) m.CreateTransform(xForm);
        }

        public static List<double[]> InterpolatePoints(this IMathUtility m, IEnumerable<double[]> pointsEnum, double stepSize)
        {
            var point2Ds = pointsEnum as IList<double[]> ?? pointsEnum.ToList();

            var interpolatedPoints =  point2Ds
                .Buffer(2,1)
                .Where(p=>p.Count==2)
                .SelectMany(p =>
                {
                    var n = (int)Math.Ceiling(m.Vector(p[0],p[1]).GetLength() / stepSize);
                    n = Math.Max(2, n);
                    return Sequences.LinSpace(p[0].ToList(), p[1].ToList(), n, false)
                        .Select(l=>l.ToArray())
                        .ToList();
                })
                .ToList();

            interpolatedPoints.Add(point2Ds.Last());

            interpolatedPoints = interpolatedPoints.Distinct().ToList();

            return interpolatedPoints;
        }
    }
}
