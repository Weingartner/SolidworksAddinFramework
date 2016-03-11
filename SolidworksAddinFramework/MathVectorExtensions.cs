using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
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

        public static double[] Cross(this IMathUtility math, double[] a, double[] b)
        {

            var v0 = (IMathVector) math.CreateVector(a);
            var v1 = (IMathVector) math.CreateVector(b);
            return (double[]) ((IMathVector)v0.Cross(v1)).ArrayData;

        }
        public static MathVector ScaleTs(this IMathVector a, double b)
        {
            return (MathVector) a.Scale(b);
        }

        public static MathVector CrossTs(this IMathVector a, IMathVector b)
        {
            return (MathVector) a.Cross(b);
        }

        public static MathVector MultiplyTransformTs(this IMathVector v, IMathTransform t)
        {
            return (MathVector) v.MultiplyTransform(t);
        }


    }
}