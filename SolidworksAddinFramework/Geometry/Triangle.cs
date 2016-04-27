using System.Numerics;

namespace SolidworksAddinFramework.Geometry
{
    /// <summary>
    /// </summary>
    public struct Edge3
    {
        public readonly Vector3 A;
        public readonly Vector3 B;

        public double Length => Delta.Length();

        public Edge3(Vector3 a, Vector3 b)
        {
            A = a;
            B = b;
        }

        public Vector3 Delta => B - A;

        public Edge3 ApplyTransform(Matrix4x4 transform) => 
            new Edge3(Vector3.Transform(A, transform), Vector3.Transform(B, transform));
    }
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