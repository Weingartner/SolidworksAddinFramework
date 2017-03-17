using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.DoubleNumerics;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;

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
        public static bool Equals(this Vector3 a, Vector3 b, double tol)
        {
            return (a - b).LengthSquared() < tol*tol;
        }

        public static MathPoint AddTs(this IMathPoint a, IMathVector b)
        {
            return (MathPoint) a.AddVector(b);
        }

        public static Vector3 ToVector3(this IMathPoint a) => 
            a.ArrayData.CastArray<double>().ToVector3();

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

        public static MathPoint MultiplyTransformTs(this IMathPoint v, IMathTransform t)
        {
            return (MathPoint) v.MultiplyTransform(t);
        }

        public static double Distance(this DenseVector a, DenseVector b)
        {
            return (a - b).L2Norm();
        }

        /// <summary>
        /// Returns the angle in the xy plane between 0 and 2 Pi
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static  double Angle2D(this IMathPoint p)
        {
            var pData = p.ArrayData.CastArray<double>();
            var angle = Math.Atan2(pData[1], pData[0]);
            return angle < 0.0 ? angle + 2*Math.PI : angle; 
        }



    }
}
