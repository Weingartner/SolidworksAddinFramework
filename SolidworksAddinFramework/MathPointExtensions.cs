using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
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
        public static MathVector SubtractTs(this IMathVector a, IMathVector b)
        {
            return (MathVector)a.Subtract(b);
        }
        public static bool Equals(this IMathPoint a, IMathPoint b, double tol)
        {
            var v = a.SubtractTs(b);
            return v.GetLength() < tol;
        }
        public static bool Equals(this IMathVector a, IMathVector b, double tol)
        {
            var v = a.SubtractTs(b);
            return v.GetLength() < tol;
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

        public static double Distance(double[] a, double[] b)
        {
            var av = new DenseVector(a);
            var bv = new DenseVector(b);
            return (av - bv).L2Norm();
        }
    }
}
