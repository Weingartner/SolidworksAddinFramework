using System.DoubleNumerics;
using System.Text;

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

        public static Matrix4x4 ExtractRotationPart(this Matrix4x4 m)
        {
            Vector3 dScale;
            Quaternion dRotation;
            Vector3 dTranslation;
            Matrix4x4.Decompose(m, out dScale, out dRotation, out dTranslation);
            return Matrix4x4.CreateFromQuaternion(dRotation);
            
        }


    }
}