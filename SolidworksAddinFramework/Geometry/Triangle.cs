using System.Numerics;

namespace SolidworksAddinFramework.Geometry
{
    public struct Triangle
    {
        public readonly Vector3 A;
        public readonly Vector3 B;
        public readonly Vector3 C;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            A = a;
            B = b;
            C = c;
        }
        public Triangle ApplyTransform(Matrix4x4 matrix)
        {
            return new Triangle(
                Vector3.Transform(A,matrix),
                Vector3.Transform(B,matrix),
                Vector3.Transform(C,matrix)
                );
        }

        public static Vector3 TriNorm(Triangle tri)
        {
            var v0 = tri.A - tri.B;
            var v1 = tri.A - tri.C;
            return Vector3.Cross(v0,v1);
        }
    }
}