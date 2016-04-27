using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Security.RightsManagement;
using OpenTK.Graphics.OpenGL;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl
{
    public static class Vector3Extensions
    {
        public static Vector3 ToVector3D(this double[] values)
        {
            Debug.Assert(values.Length==3);
            return new Vector3((float) values[0],(float) values[1],(float) values[2]);
        }

        public static Vector3 ToVector3D(this float[] values)
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

        public static Vector3 Unit(this Vector3 v) => v/v.Length();

        public static double[] ToDoubles(this Vector3 v) => new double[] {v.X, v.Y, v.Z};
        public static float[] ToSingles(this Vector3 v) => new float[] {v.X, v.Y, v.Z};

        public static Vector3 ToVector3(this IReadOnlyList<double> value)
        {
            Debug.Assert(value.Count==3);
            return new Vector3((float) value[0],(float) value[1],(float) value[2]);
        }
        public static Vector3 ToVector3(this IList<double> value)
        {
            Debug.Assert(value.Count==3);
            return new Vector3((float) value[0],(float) value[1],(float) value[2]);
        }
        public static Vector3 ToVector3(this double[] value)
        {
            Debug.Assert(value.Length>=3);
            return new Vector3((float) value[0],(float) value[1],(float) value[2]);
        }
        public static Vector3 ToVector3(this float[] value)
        {
            Debug.Assert(value.Length==3);
            return new Vector3(value[0],value[1],value[2]);
        }

        public static Vector3 ProjectOn(this Vector3 point, Vector3 axis)
        {
            return axis*Vector3.Dot(point, axis);
        }

        public static Vector3 WithZ(this Vector3 v,float value)=>new Vector3(v.X,v.Y,value);

        /// <summary>
        /// Drops the Z value. Effectively a projection on to the XY plane
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 To2D(this Vector3 v) => new Vector2(v.X, v.Y);

        public static Vector3 XComponent(this Vector3 v)=> new Vector3(v.X);
        public static Vector3 YComponent(this Vector3 v)=> new Vector3(v.Y);
        public static Vector3 ZComponent(this Vector3 v)=> new Vector3(v.Z);

        public static MathPoint ToSwMathPoint(this Vector3 v, IMathUtility m) => m.Point(new double[] {v.X, v.Y, v.Z});
        public static MathVector ToSWVector(this Vector3 v, IMathUtility m) => m.Vector(new double[] {v.X, v.Y, v.Z});

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
            var sign = Math.Sign(Vector3.Cross(v0.To3D(), v1.To3D()).Z);
            var retn = Math.Acos(Vector2.Dot(v0, v1)/v0.Length()/v1.Length())*sign;
            return retn;

        }
    }

    public static class Matrix4x4Extensions
    {
    }
}