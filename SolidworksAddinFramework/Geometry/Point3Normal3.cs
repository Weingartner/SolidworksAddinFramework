using System.Numerics;
using SolidworksAddinFramework.OpenGl;

namespace SolidworksAddinFramework.Geometry
{
    public struct Point3Normal3
    {
        public readonly Vector3 Point;
        public readonly Vector3 Normal;

        /// <summary>
        /// Apply a tranformation to the poin and a rotation to the normal. The
        /// rotation should be consistent with the matrix. 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Point3Normal3 ApplyTransform(Matrix4x4 matrix)
        {
            return new Point3Normal3
                ( Vector3.Transform(Point, matrix)
                    , Vector3.TransformNormal(Normal, matrix));
        }

        public Point3Normal3(Vector3 point, Vector3 normal)
        {
            Point = point;
            Normal = normal;
        }

        public static implicit operator Point3Normal3(Vector3 point)
        {
            return new Point3Normal3(point, default(Vector3));
        }
        public static explicit operator Vector3(Point3Normal3 point)
        {
            return point.Point;
        }
        public void GLVertex3AndNormal3()
        {
            if(!Normal.Equals(default(System.Numerics.Vector3)))
                Normal.GLNormal3();
            Point.GLVertex3();
            
        }
    }
}