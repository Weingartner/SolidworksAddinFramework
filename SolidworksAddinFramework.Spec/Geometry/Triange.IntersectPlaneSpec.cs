using System;
using System.DoubleNumerics;
using System.Linq;
using FluentAssertions;
using FsCheck;
using WeinCadSW.Spec.FsCheck;
using static LanguageExt.Prelude;
using FsCheck.Xunit;
using Weingartner.WeinCad.Interfaces;

namespace SolidworksAddinFramework.Spec.Geometry
{
    public static class ArbsCommon
    {
        public static Gen<Vector3> Vector()
        {
            return from t in
                   Gen.Three(
                       Arb.Default.NormalFloat()
                          .Generator.Where(
                          v => Math.Abs(v.Item) > 0.1 && Math.Abs(v.Item) < 1))
                   let v = t.Map((a, b, c) => new Vector3(a.Item, b.Item, c.Item))
                   where v.Length() > 0
                   select v;
        }

        public static Gen<Triangle> Triangle(this Gen<double> floats)
        {
            return floats
                .Vector3()
                .Triangle()
                .Where
                (v =>
                 {
                     var e0 = new Edge3(v.A, v.B);
                     var e1 = new Edge3(v.A, v.C);
                     var e2 = new Edge3(v.B, v.C);

                     var minDist = 1e-2;
                     var maxDist = 1;

                     if (List(e0, e1, e2).Any(e => e.Length < minDist))
                         return false;

                     if (List(e0, e1, e2).XPaired().Any(t => t.Map((a, b) => a.AngleBetween(b) < 5d/180*Math.PI)))
                         return false;

                     return true;
                 });
        }
        
    }

    public class NotIntersecting
    {
        public static Arbitrary<Vector3> Vectors => ArbsCommon
            .Vector().ToArbitrary();

        public static Arbitrary<Triangle> Triangle() => GenFloat
            .Normal
            .Where(v => v > 0.1 && v < 10)
            .Triangle()
            .ToArbitrary();

        public static Arbitrary<Matrix4x4> Transform
            => ArbsCommon.Vector().Three().Select(v => Matrix4x4.CreateWorld(v.Item1, v.Item2, v.Item3)).ToArbitrary();
    }


    public class Intersecting
    {
        /// <summary>
        /// Sign Is Different
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool SID(double a,double b )=> (a*b) < 0;

        public static Arbitrary<Vector3> Vectors => ArbsCommon
            .Vector().ToArbitrary();

        public static Arbitrary<Triangle> Triangle() => GenFloat
            .Normal
            .Where(v => v > -10 && v < 10 && ( v < -1 || v > 1))
            .Triangle()
            .Where(t=> SID(t.A.X, t.B.X) || SID(t.A.X,t.C.X) || SID(t.B.X, t.C.X))
            .ToArbitrary();

        public static Arbitrary<Matrix4x4> Transform
            => ArbsCommon.Vector().Three().Select(v => Matrix4x4.CreateWorld(v.Item1, v.Item2, v.Item3)).ToArbitrary();
    }


    [Properties]
    public class TriangeIntersectPlaneSpec
    {

        [Property(Arbitrary = new []{typeof(NotIntersecting)})]
        public void NonIntersectingShouldBeMatched(Matrix4x4 transform, Triangle triangle)
        {
            var plane = new PointDirection3(Vector3.Zero, Vector3.UnitX);

            var ttriangle = triangle.ApplyTransform(transform);
            var pplane = plane.ApplyTransform(transform);

            var intersectionO = ttriangle.IntersectPlane(pplane);

            intersectionO.IsSome.Should().Be(false);


        }

        [Property(Arbitrary = new []{typeof(Intersecting)})]
        public void IntersectingShouldBeMatched(Matrix4x4 transform, Triangle triangle)
        {
            var plane = new PointDirection3(Vector3.Zero, Vector3.UnitX);

            var ttriangle = triangle.ApplyTransform(transform);
            var pplane = plane.ApplyTransform(transform);

            var intersectionO = ttriangle.IntersectPlane(pplane);

            intersectionO.IsSome.Should().Be(true);


        }

    }
}