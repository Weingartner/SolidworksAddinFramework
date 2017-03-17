using System.DoubleNumerics;
using OpenTK.Graphics.OpenGL;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;

namespace SolidworksAddinFramework.OpenGl
{
    public static class Vector3Extensions
    {
        public static void GLVertex3(this Vector3 v) => GL.Vertex3(v.X, v.Y, v.Z);
        public static void GLNormal3(this Vector3 v) => GL.Normal3(v.X, v.Y, v.Z);
    }
    public static class Vector3ExtensionsX
    {
        public static Vector3 ToVector3(this MathPoint value) => value.ArrayData.CastArray<double>().ToVector3();
        public static Vector3 ToVector3(this MathVector value) => value.ArrayData.CastArray<double>().ToVector3();
        public static MathPoint ToSwMathPoint(this Vector3 v) => SwAddinBase.Active.Math.Point(new[] {v.X, v.Y, v.Z});
        public static MathVector ToSWVector(this Vector3 v, IMathUtility m=null) =>(m ?? SwAddinBase.Active.Math).Vector(new[] {v.X, v.Y, v.Z});
    }

    public static class PointDirectionExtensions
    {
        public static void GLVertex3AndNormal3(this PointDirection3 @this)
        {
            if(!@this.Direction.Equals(default(Vector3)))
                @this.Direction.GLNormal3();
            @this.Point.GLVertex3();
            
        }

        public static void GLVertexAndNormal(this TriangleWithNormals @this)
        {
            @this.A.GLVertex3AndNormal3();
            @this.B.GLVertex3AndNormal3();
            @this.C.GLVertex3AndNormal3();
            
        }

    }
}
