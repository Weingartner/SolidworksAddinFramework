using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class BodyExtensionSpec : SolidWorksSpec
    {
        public BodyExtensionSpec(SwPoolFixture pool) : base(pool)
        {
        }

        private IModelDoc2 CreatePart()
        {
            var partTemplateName =
                App.GetUserPreferenceStringValue((int) swUserPreferenceStringValue_e.swDefaultTemplatePart);
            return (IModelDoc2) App.NewDocument(partTemplateName, 0, 0, 0);
        }

        private IModeler Modeler => (IModeler) App.GetModeler();

        private IMathUtility MathUtility => (IMathUtility) App.GetMathUtility();

        public static IEnumerable<object[]> DistanceBetweenBodyAndCurveTestData
        {
            get
            {
                var radius = 0.02;
                var length = 0.1;
                return new[]
                {
                    new object[] {radius, length, 0.0, 0.0, 0.0},
                    new object[] { radius, length, 1.0, 0.0, 1.0},
                    new object[] { radius, length, 1.0, radius, 1.0},
                    new object[] { radius, length, 1.0, 1 +radius, Math.Sqrt(2)},
                    new object[] { radius, length, 0, 1+radius, 1.0},
                    new object[] { radius, length, 0, -(1+radius), 1.0},
                    new object[] { radius, length, -length, 0, 0.0},
                    new object[] { radius, length, 0, 2+radius, 2.0},
                };
            }
        }



        [Theory]
        [MemberData(nameof(DistanceBetweenBodyAndCurveTestData))]
        public void CLosestPointBetweenBodyAndCurveShouldWork(double radius, double length, double xOffset, double yOffset, double expectedDistance)
        {
            //App.Visible = true;
            var modelDoc = CreatePart();

            var body = ToolAlignerPropertyManagerPage.CreateTool(radius, length, xOffset, yOffset, 0, 0, Modeler, MathUtility);
            var curve = Modeler.CreateTrimmedLine(new[] {0, 0, -1.0}, new[] {0, 0, 1.0});

            double[] ptBody;
            double[] ptCurve;
            var closestDistance = body.ClosestDistanceBetweenBodyAndCurve(curve, out ptBody, out ptCurve);

            closestDistance.Should().BeApproximately(expectedDistance, 1e-5);


            ////visualisation
            //body.DisplayTs(modelDoc, Color.Green);
            //var wireBody = curve.CreateWireBody();
            //wireBody.DisplayTs(modelDoc, Color.Red);

            //if (MathUtility.Vector(ptBody, ptCurve).GetLength() > 1e-5)
            //{
            //    var connectionLine = Modeler.CreateTrimmedLine(ptBody, ptCurve);
            //    var connectionWireBody = connectionLine.CreateWireBody();
            //    connectionWireBody.DisplayTs(modelDoc, Color.YellowGreen);
            //}
        }

    }
}
