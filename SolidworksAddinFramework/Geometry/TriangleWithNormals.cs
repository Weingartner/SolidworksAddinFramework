using System.Numerics;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.OpenGl;

namespace SolidworksAddinFramework.Geometry
{
    public struct TriangleWithNormals
    {
        public Point3Normal3 A;
        public Point3Normal3 B;
        public Point3Normal3 C;

        public TriangleWithNormals(Point3Normal3 a, Point3Normal3 b, Point3Normal3 c)
        {
            A = a;
            B = b;
            C = c;
        }

        public static implicit operator TriangleWithNormals(Triangle tri)
        {
            return new TriangleWithNormals(tri.A, tri.B,tri.C);
        }
        public static explicit operator Triangle(TriangleWithNormals tri)
        {
            return new Triangle((Vector3)tri.A,(Vector3) tri.B,(Vector3)tri.C);
        }

        public TriangleWithNormals ApplyTransform(Matrix4x4 matrix, Quaternion rotation)
        {
            return new TriangleWithNormals(
                A.ApplyTransform(matrix),
                B.ApplyTransform(matrix),
                C.ApplyTransform(matrix)
                );
        }

        public void GLVertexAndNormal()
        {
            A.GLVertex3AndNormal3();
            B.GLVertex3AndNormal3();
            C.GLVertex3AndNormal3();
            
        }

    }
}