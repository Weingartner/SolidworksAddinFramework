using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using SolidworksAddinFramework.OpenGl;
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
                yield return new object[] {new[] {0,0,0.0}, new[] {1,0,0.0}, 1e-3, 1001};
                yield return new object[] {new[] {0,0,0.0}, new[] {1,1,1.0}, 1e-3, 1734};
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
        public void InterpolatePointsShouldWork(double[]p0,double[]p1,double stepSize, int expectedPointCount)
        {
            var points = new[] {p0, p1};

            var interpolatedPoints = MathUtility.InterpolatePoints(points, stepSize);
            interpolatedPoints.Count.Should().Be(expectedPointCount);
            interpolatedPoints
                .Buffer(2, 1)
                .Where(p => p.Count == 2)
                .ForEach(p =>
                {
                    MathUtility.Vector(p[0].ToArray(), p[1].ToArray()).GetLength().Should().BeApproximately(stepSize,1e-5);
                });
        }

        [Fact]
        public void TransparentColorOpenGlShouldWork()
        {

            App.Visible = true;
            App.NewPart();
            var modeler = (IModeler) App.GetModeler();
            var modelDoc = (ModelDoc2)App.ActiveDoc;
            var docView = new DocView(App,(ModelView)modelDoc.ActiveView);
            docView.AttachEventHandlers();
            var radius = 1e-2;
            var length = 1;
            var array = new[] {0, 0, 0, 0, 0, 1, radius, length}.ToArray();
            var body = (Body2) modeler.CreateBodyFromCyl(array);
            body.Display3(modelDoc, 255, 1);
            //var newToolMesh = new Mesh(body);
            //newToolMesh.DisplayUndoable(modelDoc, Color.FromArgb(200, Color.Yellow));
            var foo = 6;
        }

    }
}
