using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using Weingartner.Numerics;
using Xunit;
using XUnit.Solidworks.Addin;


namespace SolidworksAddinFramework.Spec
{
    public class ModelerExtensionSpec : SolidWorksSpec
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
            CreatePartDoc(false, modelDoc =>
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

        [SolidworksFact]
        public void InterpolateCurveShouldWork()
        {
            CreatePartDoc(false, modelDoc =>
            {
                var points = new[]
                {
                    new Vector3(1.306213E-17f, -0.02393888f, -0.03204574f),
                    new Vector3(0f, 3.469447E-18f, 0.04f),
                    new Vector3(0.4619068f, 0.03750812f, 0.01389752f),
                    new Vector3(0.5f, 0.0386182f, -0.01042281f),
                    new Vector3(1f, 0.03395086f, -0.0211504f),
                    new Vector3(1f, -0.01454161f, 0.03726314f),
                    new Vector3(0.4766606f, 0.03750812f, 0.01389752f),
                }
                .ToList();
                var curve = Modeler.InterpolatePointsToCurve(points, 1e-2, true, false);
                var wB = curve.CreateWireBody();
                var d = wB.DisplayUndoable(modelDoc, Color.Blue);

                return new CompositeDisposable(d);
            });
        }

        [SolidworksFact]
        public void LinspaceForVector3ShouldWork()
        {
            CreatePartDoc(false, modelDoc =>
            {
                var v0 = new Vector3(0, 0, 0);
                var v1 = new Vector3(1, 0, 0);

                var intV = Sequences.LinSpace(v0, v1, 5).ToList();

                var interpolated = intV
                    .Buffer(2, 1)
                    .Where(p => p.Count == 2)
                    .Select(ps =>
                    {
                        var edge = new Edge3(ps[0], ps[1]);
                        var limitedLine = Modeler.CreateTrimmedLine(edge);
                        var wB = limitedLine.CreateWireBody();
                        return wB.DisplayUndoable(modelDoc, Color.Blue);
                    })
                    .ToCompositeDisposable();

                return new CompositeDisposable(interpolated);
            });
        }

        
    }

    public class Edge3Spec
    {
        Vector3 vector(double x, double y, double z)=>new Vector3((float) x,(float) y,(float) z);

        [Fact]
        public void ShouldGetTheShortestLine()
        {
            var edge0 = new Edge3(vector(0,-1,-1), vector(0,1,-1));
            var edge1 = new Edge3(vector(-1,0,1), vector(1,0,1));

            var connect = edge0.ShortestEdgeJoining(edge1);

            connect.A.X.Should().BeApproximately(0, 1e-6f);
            connect.A.Y.Should().BeApproximately(0, 1e-6f);
            connect.A.Z.Should().BeApproximately(-1, 1e-6f);

            connect.B.X.Should().BeApproximately(0, 1e-6f);
            connect.B.Y.Should().BeApproximately(0, 1e-6f);
            connect.B.Z.Should().BeApproximately(1, 1e-6f);

        }
        
    }
}
