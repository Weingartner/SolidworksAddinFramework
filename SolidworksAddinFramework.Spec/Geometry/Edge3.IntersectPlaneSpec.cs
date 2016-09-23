using System;
using System.CodeDom;
using System.DoubleNumerics;
using FluentAssertions;
using FsCheck;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.Spec.FluentAssertions;
using Xunit;
using FsCheck.Xunit;

namespace SolidworksAddinFramework.Spec.Geometry
{
    [Properties(Arbitrary = new[] { typeof(Arbitraries) })]
    public class Edge3IntersectPlaneSpec {

        private static class Arbitraries
        {
            public static Arbitrary<Vector3> VectorArbitrary()
            {
                var gen =
                    from t in
                        Gen.Three(
                            Arb.Default.NormalFloat()
                                .Generator.Where(
                                    v => Math.Abs(v.Item) > 0.1 && Math.Abs(v.Item) < 1))
                    let v = t.Map((a, b, c) => new Vector3(a.Item, b.Item, c.Item))
                    where v.Length() > 0
                    select v;
                return gen.ToArbitrary();
            }
        }

        [Property]
        public void IntersectingLineWithPlaneShouldReturnIntersectionPoint(Vector3 position, Vector3 forward, Vector3 up)
        {
            var line = new Edge3(new Vector3(1,2,3), new Vector3(-1,-2,-3));
            var plane = new PointDirection3(Vector3.Zero, Vector3.UnitX);

            var transform = Matrix4x4.CreateWorld(position, forward, up);

            var tline = line.ApplyTransform(transform);
            var tplane = plane.ApplyTransform(transform);

            var intersectPlane = tline.IntersectPlane(tplane).__Value__();
            var intersectPlaneExpectedValue = Vector3.Transform(Vector3.Zero, transform);
            intersectPlane.Should().BeApproximately(intersectPlaneExpectedValue, 1e-6);
        }

        [Property]
        public void NonIntersectingLinesShouldReturnNone(Vector3 position, Vector3 forward, Vector3 up)
        {
            var line = new Edge3(new Vector3(1, 2, 3), new Vector3(1, 2, 3) * 0.001);
            var plane = new PointDirection3(Vector3.Zero, Vector3.UnitX);

           var transform = Matrix4x4.CreateWorld(position, forward, up);

           var tline = line.ApplyTransform(transform);
           var tplane = plane.ApplyTransform(transform);

           tline.IntersectPlane(tplane).IsSome.Should().BeFalse();
        }

        [Property]
        public void NonIntersectingLinesShouldReturnNone_v2(Vector3 position, Vector3 forward, Vector3 up)
        {
            var line = new Edge3(new Vector3(-1, -2, -3) * 0.001, new Vector3(-1, -2, -3));
            var plane = new PointDirection3(Vector3.Zero, Vector3.UnitX);

           var transform = Matrix4x4.CreateWorld(position, forward, up);

           var tline = line.ApplyTransform(transform);
           var tplane = plane.ApplyTransform(transform);

           tline.IntersectPlane(tplane).IsSome.Should().BeFalse();
        }
    }
}