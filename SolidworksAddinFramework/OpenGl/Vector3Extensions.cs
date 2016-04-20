using System.Diagnostics;
using System.Numerics;
using OpenTK.Graphics.OpenGL;

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
    }

    public static class Matrix4x4Extensions
    {
    }
}