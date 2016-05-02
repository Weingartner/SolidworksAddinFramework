using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using Xunit;
using XUnit.Solidworks.Addin;


namespace SolidworksAddinFramework.Spec
{
    public class CreateTrimmedLineSpec : SolidWorksSpec
    {
        private static IModeler Modeler => (IModeler)SwApp.GetModeler();
        private static IMathUtility MathUtility => (IMathUtility)SwApp.GetMathUtility();


        public static IEnumerable<object[]> TrimLineTestData => new[]
        {
            new object[] {new[] {0f, 0f, 0f}, new[] {1f, 1f, 1f}},
            new object[] {new[] { 2.92671522E-17f, -0.251327425f, 0.0f }, new[] { 1.56720257f, 1.06371164f, 0.0f }},
            new object[] {new[] {1.7f, 1.426469f, 0f}, new[] {1.7f, 1.175142f, 0f}},
        };

        [SolidworksTheory]
        [MemberData(nameof(TrimLineTestData))]
        public void TrimLineShouldWork
            (float[] p0
            , float[] p1)
        {
            CreatePartDoc(true, modelDoc =>
            {
                var v0 = new Vector3(p0[0], p0[1], p0[2]);
                var v1 = new Vector3(p1[0], p1[1], p1[2]);
                var edge = new Edge3(v0, v1);

                var limitedLine = Modeler.CreateTrimmedLine(edge);

                var tol = 1e-9f;
                var startPoint = limitedLine.StartPoint(0)[0];
                var endPoint = limitedLine.EndPoint(0)[0];
                v0.X.Should().BeApproximately(startPoint.X, tol);
                v0.Y.Should().BeApproximately(startPoint.Y, tol);
                v0.Z.Should().BeApproximately(startPoint.Z, tol);
                v1.X.Should().BeApproximately(endPoint.X, tol);
                v1.Y.Should().BeApproximately(endPoint.Y, tol);
                v1.Z.Should().BeApproximately(endPoint.Z, tol);

                //######################################################
                var wbv = limitedLine.CreateWireBody();
                var d = wbv.DisplayUndoable(modelDoc, Color.Blue);
                return d;
            });
        }


    }
}
