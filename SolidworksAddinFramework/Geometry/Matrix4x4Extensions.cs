using System;
using System.Diagnostics;
using System.DoubleNumerics;
using System.Text;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;

namespace SolidworksAddinFramework.Geometry
{
    public static class Matrix4x4Extensions
    {
        public static Matrix4x4 CreateFromAxisAngleOrigin(PointDirection3 p, double angle)
        {
            return
                Matrix4x4.CreateTranslation(-p.Point)
                *Matrix4x4.CreateFromAxisAngle(p.Direction.Unit(), angle)
                *Matrix4x4.CreateTranslation(p.Point);
        }
        public static Matrix4x4 CreateFromEdgeAngleOrigin(Edge3 p, double angle)
        {
            return CreateFromAxisAngleOrigin(new PointDirection3(p.A,p.Delta),angle);
        }

        /// <summary>
        /// A fluent matrix inversion. Will fail with an
        /// exception if the matrix is not invertable. Use
        /// only when you are sure you have an invertable 
        /// matrix;
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix4x4 InvertUnsafe(this Matrix4x4 m)
        {
            Matrix4x4 inv;
            if (!Matrix4x4.Invert(m, out inv))
            {
                throw new Exception("Matrix inversion failed");
            }
            return inv;

        }

        public static Matrix4x4 ExtractRotationPart(this Matrix4x4 m)
        {
            Vector3 dScale;
            Quaternion dRotation;
            Vector3 dTranslation;
            Matrix4x4.Decompose(m, out dScale, out dRotation, out dTranslation);
            return Matrix4x4.CreateFromQuaternion(dRotation);
            
        }

        public static MathTransform ToSwTransform(this Matrix4x4 m, IMathUtility math = null) => 
            (math ?? SwAddinBase.Active.Math).ToSwMatrix(m);
    }
}