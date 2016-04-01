using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SwCSharpAddinSpecHelper;
using WeinCadSW.Macros.ToolAligner;
using Xunit;
using Xunit.Extensions;

namespace SolidworksAddinFramework.Spec
{
    public class CurveExtensionSpec : SolidWorksSpec
    {
        private IModeler Modeler => (IModeler)App.GetModeler();

        private IMathUtility MathUtility => (IMathUtility)App.GetMathUtility();

        private IModelDoc2 CreatePart()
        {
            var partTemplateName = App.GetUserPreferenceStringValue((int) swUserPreferenceStringValue_e.swDefaultTemplatePart);
            return (IModelDoc2)App.NewDocument(partTemplateName, 0, 0, 0);
        }
        public CurveExtensionSpec(SwPoolFixture pool) : base(pool)
        {
        }

        public static IEnumerable<object[]> ClosestPointToZTestData
        {
            get
            {
                return new[]
                {
                    new object[] {0.0, 0.0},
                    new object[] {1.0,1.0},
                    new object[] {2,1},
                    new object[] {1.5,1},
                    new object[] {-1.5,-1},
                    new object[] {-2,-1},
                    new object[] {-1,-1},
                };
            }
        }

        [Theory]
        [MemberData(nameof(ClosestPointToZTestData))]
        public void ClosestPointTozPositionShouldWork(double zPosition, double expectedZPosition)
        {
            //App.Visible = true;

            var modelDoc = CreatePart();

            var curve = Modeler.CreateTrimmedLine(new[] {0, 0, -1.0}, new[] {0, 0, 1.0});
            var closestPoint = curve.ClosestPointToZPosition(zPosition);
            closestPoint[2].Should().BeApproximately(expectedZPosition, 1e-5);
        }
    }
}
