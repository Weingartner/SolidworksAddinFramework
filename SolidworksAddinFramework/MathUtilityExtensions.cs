using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        /// <summary>
        ///  Transformation matrix data:
        ///
        ///    |a b c.n |
        ///    |d e f.o |
        ///    |g h i.p |
        ///    |j k l.m |
        ///
        /// The SOLIDWORKS transformation matrix is stored as a homogeneous matrix of 16 elements, ordered as shown.The first 9 elements(a to i) are elements of a 3x3 rotational sub-matrix, the next 3 elements(j, k, l) define a translation vector, and the next 1 element(m) is a scaling factor.The last 3 elements(n, o, p) are unused in this context.
        ///
        /// The 3x3 rotational sub-matrix represents 3 axis sets:
        ///
        /// row 1 for x-axis components of rotation
        /// row 2 for y-axis components of rotation
        /// row 3 for z-axis components of rotation
        /// The 3 axes are constrained to be orthogonal and unified so that they
        /// produce a pure rotational transformation.Reflections can also be added
        /// to these axes by setting the components to negative.The rotation sub-matrix
        /// coupled with the lower-left translation vector and the lower-right corner scaling
        /// factor creates an affine transformation, which is a transformation that preserves lines
        /// and parallelism; i.e., maps parallel lines to parallel lines.
        ///
        /// If the 3 axis sets of the 3x3 rotational sub-matrix are not orthogonal or unified,
        /// then they are automatically corrected according to the following rules:
        ///
        ///
        /// If any axis is 0, or any two axes are parallel, or all axes are coplanar, then an identity matrix replaces the rotational sub-matrix.
        ///
        /// All axes are corrected to be of unit length.
        ///
        /// The axes are built to be orthogonal to each other in the prioritized order of Z, X, Y (X is orthogonal to Z, Y is orthogonal to Z and X).
        /// </summary>
        /// <param name=""></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static MathTransform ToSwMatrix(this IMathUtility math , Matrix4x4 matrix)
        {
            var _ = matrix;
            double a = _.M11, b = _.M12, c = _.M13, n = _.M14;
            double d = _.M21, e = _.M22, f = _.M23, o = _.M24;
            double g = _.M31, h = _.M32, i = _.M33, p = _.M34;
            double j = _.M41, k = _.M42, l = _.M43, m = _.M44;

            double[] data = new[] {a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p};

            return (MathTransform) math.CreateTransform(data);
        }

        public static Matrix4x4 ToMatrix4X4(this IMathTransform transform)
        {
            var _ =  new Matrix4x4();
            var array = transform.ArrayData.CastArray<float>();
            var a = array[0];
            var b = array[1];
            var c = array[2];
            var d = array[3];
            var e = array[4];
            var f = array[5];
            var g = array[6];
            var h = array[7];
            var i = array[8];
            var j = array[9];
            var k = array[10];
            var l = array[11];
            var m = array[12];
            var n = array[13];
            var o = array[14];
            var p = array[15];

            _.M11 = a; _.M12 = b; _.M13 = c; _.M14 = n;
            _.M21 = d; _.M22 = e; _.M23 = f; _.M24 = o;
            _.M31 = g; _.M32 = h; _.M33 = i; _.M34 = p;
            _.M41 = j; _.M42 = k; _.M43 = l; _.M44 = m;

            return _;
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
