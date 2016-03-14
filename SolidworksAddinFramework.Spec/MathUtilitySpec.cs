using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using SolidWorks.Interop.sldworks;
using SwCSharpAddinSpecHelper;
using Xunit;

namespace SolidworksAddinFramework.Spec
{
    public class MathUtilitySpec : SolidWorksSpec
    {
        public MathUtilitySpec(SwPoolFixture pool) : base(pool)
        {
        }

        [Fact]
        public void ShouldBeAbleToGetCrossProduct()
        {
            var mathUtility = (IMathUtility) App.GetMathUtility();
            var result = mathUtility.Cross(new [] { 1.0, 2.0, 3.0 }, new [] { 4.0, 5.0, 6.0 });
            result.ShouldAllBeEquivalentTo(new[] { -3.0, 6.0, -3.0 });
        }
    }
}
