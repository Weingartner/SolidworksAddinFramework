using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using SolidWorks.Interop.sldworks;
using SwCSharpAddinSpecHelper;
using Weingartner.Numerics;
using Xunit;

namespace SolidworksAddinFramework.Spec
{
    public class MathUtilitySpec : SolidWorksSpec
    {
        public IMathUtility MathUtility => (IMathUtility) App.GetMathUtility();

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
    }
}
