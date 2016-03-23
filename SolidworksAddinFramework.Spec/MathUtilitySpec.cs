using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using SolidWorks.Interop.sldworks;
using SwCSharpAddinSpecHelper;
using Weingartner.Numerics;
using Xunit;
using Xunit.Extensions;

namespace SolidworksAddinFramework.Spec
{
    public class MathUtilitySpec : SolidWorksSpec
    {
        public IMathUtility MathUtility => (IMathUtility) App.GetMathUtility();

        public static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new object[] {1.0};
            }
        }

        public MathUtilitySpec(SwPoolFixture pool) : base(pool)
        {
        }

        [Fact]
        public void ShouldBeAbleToGetCrossProduct()
        {
            var result = MathUtility.Cross(new [] { 1.0, 2.0, 3.0 }, new [] { 4.0, 5.0, 6.0 });
            result.ShouldAllBeEquivalentTo(new[] { -3.0, 6.0, -3.0 });
        }


        [Fact]
        public void RotateByAngleShouldWork()
        {
            var p = MathUtility.Point(new[] {1, 0.0, 0});
            var zAxis = MathUtility.Vector(new[] {0, 0, 1.0});

            Sequences.LinSpace(0, 2*Math.PI, 10, false)
                .ToList()
                .ForEach(a =>
                {
                    var newPoint = MathUtility.RotateByAngle(p, zAxis, a);
                    newPoint.Angle2D().Should().BeApproximately(a, 1e-5);
                });
        }

        [Theory, MemberData(nameof(TestData))]
        public void InterpolatePointsShouldWork(double foo)
        {
            var p0 = new[] {0, 0, 0.0};
            var p1 = new[] {1, 0, 0.0};
            var points = new[] {p0, p1};
            var stepSize = 1e-3;

            var interpolatedPoints = MathUtility.InterpolatePoints(points, stepSize);
            interpolatedPoints.Count.Should().Be(1001);
            interpolatedPoints
                .Buffer(2, 1)
                .Where(p => p.Count == 2)
                .ForEach(p =>
                {
                    MathUtility.Vector(p[0].ToArray(), p[1].ToArray()).GetLength().Should().BeApproximately(stepSize,1e-5);
                });
        }

        [Fact]
        public void InterpolatePointsShouldWork2()
        {
            var p0 = new[] {0, 0, 0.0};
            var p1 = new[] {1, 1, 1.0};
            var points = new[] {p0, p1};
            var stepSize = 1e-3;

            var interpolatedPoints = MathUtility.InterpolatePoints(points, stepSize);
            interpolatedPoints.Count.Should().Be(1734);
            interpolatedPoints
                .Buffer(2, 1)
                .Where(p => p.Count == 2)
                .ForEach(p =>
                {
                    MathUtility.Vector(p[0].ToArray(), p[1].ToArray()).GetLength().Should().BeApproximately(stepSize,1e-5);
                });

        }
    }
}
