using System.Collections.Generic;
using System.DoubleNumerics;

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

        public IEnumerable<Triangle> Sweep(Triangle other)
        {
            var x0 = this;
            var x1 = other;
            yield return new Triangle(x0.A, x1.C, x0.C);
            yield return new Triangle(x0.A, x1.A, x0.C);
            yield return new Triangle(x0.A, x1.A, x0.B);
            yield return new Triangle(x0.B, x1.B, x1.A);
            yield return new Triangle(x0.B, x1.B, x0.C);
            yield return new Triangle(x0.C, x1.C, x1.B);
        }
    }
}