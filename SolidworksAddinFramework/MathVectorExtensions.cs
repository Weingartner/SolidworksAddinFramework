using System;
using System.Diagnostics;
using System.Linq;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;
using DLA = MathNet.Numerics.LinearAlgebra.Double;

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


        /// <summary>
        /// Cross product for 3D vectors
        /// http://stackoverflow.com/a/20015626/158285
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DLA.DenseVector Cross(this DLA.DenseVector left, DLA.DenseVector right)
        {
            if ((left.Count != 3 || right.Count != 3))
            {
                string message = "Vectors must have a length of 3.";
                throw new Exception(message);
            }
            var result = new DLA.DenseVector(3);
            result[0] = left[1] * right[2] - left[2] * right[1];
            result[1] = -left[0] * right[2] + left[2] * right[0];
            result[2] = left[0] * right[1] - left[1] * right[0];

            return result;
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

        public static MathVector AlphaBend(this IMathVector a, IMathVector b, double alpha)
        {
            Debug.Assert(alpha>=0);
            Debug.Assert(alpha<=1);
            return (MathVector) a.ScaleTs(alpha).Add(b.ScaleTs(1 - alpha));
        }

        public static MathVector CrossTs(this IMathVector a, IMathVector b)
        {
            return (MathVector) a.Cross(b);
        }

        public static MathVector MultiplyTransformTs(this IMathVector v, IMathTransform t)
        {
            return (MathVector) v.MultiplyTransform(t);
        }

        public static double LengthOfProjectionXY(this double[] vector)
        {
            return Math.Sqrt(vector.Take(2).Sum(c => Math.Pow(c, 2)));
        }

        public static double AngleBetweenVectors(this IMathVector v0, IMathVector v1)
        {
            if (((double[])v0.ArrayData).Length==((double[])v1.ArrayData).Length)
            {
                var sign = Math.Sign(((IMathVector)(v0.Cross(v1))).ArrayData.CastArray<double>()[2]);
                var ret = Math.Acos(v0.Dot(v1) / (v0.GetLength() * v1.GetLength()));
                return sign * ret;
            }
            throw new Exception("Vectors must have the same dimension!");
        }
    }
}