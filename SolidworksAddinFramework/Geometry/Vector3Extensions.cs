using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DoubleNumerics;
using OpenTK.Graphics.OpenGL;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.Geometry
{
    public static class Vector3Extensions
    {

        /// <summary>
        /// Performs the perspective transformation to turn a 4D homogeneos coordinate into
        /// Euclidian coordinates. Useful for dealing with b-splines
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 ToEuclid(this Vector4 v)=> new Vector3(v.X/v.W,v.Y/v.W,v.Z/v.W);

        /// <summary>
        /// Converts a euclidian vector into homogeneous space. Use ToEuclid to get it back
        /// again. Useful in dealing with b-splines
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static Vector4 ToHomogenous(this Vector3 v, double w) => new Vector4(v*w,w);

        public static Vector3 ToVector3D(this double[] values)
        {
            Debug.Assert(values.Length==3);
            return new Vector3(values[0],values[1],values[2]);
        }

        public static void GLVertex3(this Vector3 v)
        {
            GL.Vertex3(v.X, v.Y, v.Z);
        }
        public static void GLNormal3(this Vector3 v)
        {
            GL.Normal3(v.X, v.Y, v.Z);
        }

        public static Vector3 Unit(this Vector3 v) => Vector3.Normalize(v);

        public static double Dot(this Vector3 a, Vector3 other)
        {
            return Vector3.Dot(a, other);
        }

        /// <summary>
        /// Returns 1 for right handed vectors and -1 for left handed
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int Orientation(this Vector3 a, Vector3 b, Vector3 c) =>
            Vector3.Cross(a, b).Dot(c) > 0 ? 1 : -1;

        /// <summary>
        /// Returns an orthoganl vector. This is robust
        /// From http://lolengine.net/blog/2013/09/21/picking-orthogonal-vector-combing-coconuts
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 Orthogonal(this Vector3 v) =>
            Math.Abs(v.X) > Math.Abs(v.Z) ? new Vector3(-v.Y, v.X, 0.0) : new Vector3(0.0, -v.Z, v.Y); 

        public static double[] ToDoubles(this Vector3 v) => new[] {v.X, v.Y, v.Z};
        public static double[] ToDoubles(this Vector4 v) => new[] {v.X, v.Y, v.Z, v.W};
        public static double[] ToSingles(this Vector3 v) => new[] {v.X, v.Y, v.Z};

        public static Vector3 ToVector3(this IReadOnlyList<double> value)
        {
            Debug.Assert(value.Count==3);
            return new Vector3(value[0],value[1],value[2]);
        }
        public static Vector3 ToVector3(this IList<double> value)
        {
            Debug.Assert(value.Count==3);
            return new Vector3(value[0],value[1],value[2]);
        }
        public static Vector3 ToVector3(this MathPoint value)
        {
            return value.ArrayData.CastArray<double>().ToVector3();
        }
        public static Vector3 ToVector3(this MathVector value)
        {
            return value.ArrayData.CastArray<double>().ToVector3();
        }

        public static Vector3 ToVector3(this double[] value)
        {
            Debug.Assert(value.Length==3);
            return new Vector3(value[0],value[1],value[2]);
        }

        public static Vector3 ProjectOn(this Vector3 point, Vector3 axis)
        {
            return ProjectOnUnit(point, axis.Unit());
        }

        public static Vector3 ProjectOnUnit(this Vector3 point, Vector3 axis)
        {
            return axis*Vector3.Dot(point, axis);
        }

        public static Vector3 WithZ(this Vector3 v,double value)=>new Vector3(v.X,v.Y,value);

        /// <summary>
        /// Drops the Z value. Effectively a projection on to the XY plane
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 To2D(this Vector3 v) => new Vector2(v.X, v.Y);

        public static bool Equals(this Vector3 a, Vector3 other, double tol) => 
            (a - other).LengthSquared() < tol*tol;

        public static Vector3 XComponent(this Vector3 v)=> new Vector3(v.X,0,0);
        public static Vector3 YComponent(this Vector3 v)=> new Vector3(0,v.Y,0);
        public static Vector3 ZComponent(this Vector3 v)=> new Vector3(0,0,v.Z);

        public static MathPoint ToSwMathPoint(this Vector3 v) => SwAddinBase.Active.Math.Point(new[] {v.X, v.Y, v.Z});
        public static MathVector ToSWVector(this Vector3 v, IMathUtility m) => m.Vector(new[] {v.X, v.Y, v.Z});

        /// <summary>
        /// Converts to Vector3 and sets the z component to 0.0
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 To3D(this Vector2 v) => new Vector3(v.X, v.Y,0);


        /// <summary>
        /// returns the signed angle between the two vectors
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double SignedAngle(this Vector2 v0, Vector2 v1)
        {
            var v0Length = v0.Length();
            if (v0Length == 0)
            {
                throw new ArgumentException("Length must be greater than 0.", nameof(v0));
            }
            var v1Length = v1.Length();
            if (v1Length == 0)
            {
                throw new ArgumentException("Length must be greater than 0.", nameof(v1));
            }

            var sign = Math.Sign(Vector3.Cross(v0.To3D(), v1.To3D()).Z);
            var acos = Math.Acos(Vector2.Dot(v0, v1) / v0Length / v1Length);
            return Double.IsNaN(acos) ? 0 : acos * sign;
        }

        public static double AngleBetweenVectors(this Vector3 v0, Vector3 v1)
        {
            return Math.Acos(v0.Dot(v1)/(v0.Length()*v1.Length()));
        }
    }
}