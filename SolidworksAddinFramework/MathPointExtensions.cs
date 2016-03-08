using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class MathPointExtensions
    {
        public static MathPoint Project(this IMathPoint point, IMathPoint origin, IMathVector axis)
        {
            var a = (IMathVector) point.Subtract(origin);
            var t = a.Project(axis);
            var v = (MathVector) axis.Scale(t);
            return (MathPoint) origin.AddVector(v);
        }

        public static MathVector SubtractTs(this IMathPoint a, IMathPoint b)
        {
            return (MathVector)a.Subtract(b);
        }

        public static MathPoint AddTs(this IMathPoint a, IMathVector b)
        {
            return (MathPoint) a.AddVector(b);
        }

        public static double[] Average(this IEnumerable<double[]> points)
        {
            var list = points.ToList();
            Debug.Assert(list.Select(a=>a.Length).Distinct().Count()==1);
            var length = list[0].Length;
            var output = new double[length];
            for (int i = 0; i < length; i++)
            {
                output[i] = list.Select(a => a[i]).Sum()/length;
            }
            return output;
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
    }
}
