using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Linq;
using System.Runtime.InteropServices;
using LanguageExt;

namespace SolidworksAddinFramework.Geometry
{
    public struct Triangle
    {
        public readonly Vector3 A;
        public readonly Vector3 B;
        public readonly Vector3 C;

        public Edge3 EdgeAB => new Edge3(A,B);
        public Edge3 EdgeBC => new Edge3(B,C);
        public Edge3 EdgeCA => new Edge3(C,A);

        public override string ToString()
        {
            return $"[{A}, {B}, {C}]";
        }

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

        private static Option<Edge3> Match(Option<Vector3> t0, Option<Vector3> t1)
        {
            return (from a in t0
                    from b in t1
                    select new Edge3(a, b));
        }

        public Option<Edge3> IntersectPlane(PointDirection3 plane)
        {
            var d = plane.IsAbovePlane(A);
            if (d == plane.IsAbovePlane(B) && d == plane.IsAbovePlane(C))
                return Option<Edge3>.None;


            var p0 = EdgeAB.IntersectPlane(plane);
            var p1 = EdgeBC.IntersectPlane(plane);
            var p2 = EdgeCA.IntersectPlane(plane);

            return Match(p0, p1)
                .BindNone(() => Match(p0, p2))
                .BindNone (() => Match(p1, p2));

        }

        private static Edge3 Edge3(Vector3 a, Vector3 b)
        {
            return new Edge3(a, b);
        }

        public Vector3 DropVectorC()
        {
            var vBC = C - B;
            var vBA = A - B;
            var vDropC = vBC.ProjectOn(vBA);
            var k = vBC-vDropC;
            return k;
        }
    }
}