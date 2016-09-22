using System;
using System.DoubleNumerics;
using FluentAssertions;
using FsCheck;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.Spec.FluentAssertions;
using Xunit;

namespace SolidworksAddinFramework.Spec.Geometry
{
    public class Edge3IntersectPlaneSpec {

        public static Gen<Vector3> VectorGen
        {
            get
            {
                return
                    from t in
                        Gen.Three(
                            Arb.Default.NormalFloat()
                                .Generator.Where(v => Math.Abs(v.Item) > 0.1 && Math.Abs(v.Item) < 1))
                    let v = t.Map((a, b, c) => new Vector3(a.Item, b.Item, c.Item))
                    where v.Length() > 0
                    select v
                    ;
            }
        }

        [Fact]
        public void IntersectingLineWithPlaneShouldReturnIntersectionPoint()
        {
            var line = new Edge3(new Vector3(1,2,3), new Vector3(-1,-2,-3));
            var plane = new PointDirection3(Vector3.Zero, Vector3.UnitX);

            var varb = VectorGen.ToArbitrary();
            Prop.ForAll(varb,varb,varb,(position,forward,up)=>
            {
                var transform = Matrix4x4.CreateWorld(position, forward, up);

                var tline = line.ApplyTransform(transform);
                var tplane = plane.ApplyTransform(transform);

                var intersectPlane = tline.IntersectPlane(tplane).__Value__();
                var intersectPlaneExpectedValue = Vector3.Transform(Vector3.Zero, transform);

                intersectPlane.Should().BeApproximately(intersectPlaneExpectedValue, 1e-6);
            })
                .QuickCheckThrowOnFailure();
        }

        [Fact]
        public void NonIntersectingLinesShouldReturnNone()
        {
            var line = new Edge3(new Vector3(1,2,3), new Vector3(1,2,3)*0.001);
            var plane = new PointDirection3(Vector3.Zero, Vector3.UnitX);

            var varb = VectorGen.ToArbitrary();
            Prop.ForAll(varb,varb,varb,(position,forward,up)=>
            {
                var transform = Matrix4x4.CreateWorld(position, forward, up);

                var tline = line.ApplyTransform(transform);
                var tplane = plane.ApplyTransform(transform);

                tline.IntersectPlane(tplane).IsSome.Should().BeFalse();
            })
                .QuickCheckThrowOnFailure();
        }

        [Fact]
        public void NonIntersectingLinesShouldReturnNone_v2()
        {
            var line = new Edge3(new Vector3(-1,-2,-3)*0.001, new Vector3(-1,-2,-3));
            var plane = new PointDirection3(Vector3.Zero, Vector3.UnitX);

            var varb = VectorGen.ToArbitrary();
            Prop.ForAll(varb,varb,varb,(position,forward,up)=>
            {
                var transform = Matrix4x4.CreateWorld(position, forward, up);

                var tline = line.ApplyTransform(transform);
                var tplane = plane.ApplyTransform(transform);

                tline.IntersectPlane(tplane).IsSome.Should().BeFalse();
            })
                .QuickCheckThrowOnFailure();
        }
    }
}