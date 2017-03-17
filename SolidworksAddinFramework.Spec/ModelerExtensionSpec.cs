using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.DoubleNumerics;
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
using Weingartner.WeinCad.Interfaces;
using Xunit;
using XUnit.Solidworks.Addin;


namespace SolidworksAddinFramework.Spec
{
    public class ModelerExtensionSpec : SolidWorksSpec
    {
        public static IEnumerable<object[]> TrimLineTestData => new[]
        {
            new object[] {new[] {0, 0, 0}, new[] {1, 1, 1}},
            new object[] {new[] { 2.92671522E-17, -0.251327425, 0.0 }, new[] { 1.56720257, 1.06371164, 0.0 }},
            new object[] {new[] {1.7, 1.426469, 0}, new[] {1.7, 1.175142, 0}},
        };

        [SolidworksFact]
        public void CreateArcShouldWork()
        {
            var arc = (Curve) Modeler.CreateTrimmedArc (Vector3.Zero, Vector3.UnitZ, 10 * Vector3.UnitX, -10 * Vector3.UnitX);

            bool isPeriodic;
            double end;
            bool isClosed;
            double start;
            arc.GetEndParams(out start, out end, out isClosed, out isPeriodic);

            start.Should().Be(0);

            var startArray = (double[]) arc.Evaluate2(start, 0);
            var endArray = (double[]) arc.Evaluate2(end, 0);

            startArray[0].Should().BeApproximately(10,1e-5);
            startArray[1].Should().BeApproximately(0,1e-5);
            startArray[1].Should().BeApproximately(0,1e-5);

            endArray[0].Should().BeApproximately(-10,1e-5); // This fails
            endArray[1].Should().BeApproximately(0,1e-5);
            endArray[1].Should().BeApproximately(0,1e-5);

            isClosed.Should().BeFalse(); // This fails

        }

        [SolidworksTheory]
        [MemberData(nameof(TrimLineTestData))]
        public void TrimLineShouldWork
            (double[] p0
            , double[] p1)
        {
            CreatePartDoc(modelDoc =>
            {
                var v0 = new Vector3(p0[0], p0[1], p0[2]);
                var v1 = new Vector3(p1[0], p1[1], p1[2]);
                var edge = new Edge3(v0, v1);

                var limitedLine = Modeler.CreateTrimmedLine(edge);

                var tol = 1e-9;
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
            CreatePartDoc(modelDoc =>
            {
                var points = new[]
                {
                    new Vector3(1.306213E-17, -0.02393888, -0.03204574),
                    new Vector3(0, 3.469447E-18, 0.04),
                    new Vector3(0.4619068, 0.03750812, 0.01389752),
                    new Vector3(0.5, 0.0386182, -0.01042281),
                    new Vector3(1, 0.03395086, -0.0211504),
                    new Vector3(1, -0.01454161, 0.03726314),
                    new Vector3(0.4766606, 0.03750812, 0.01389752),
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
            CreatePartDoc(modelDoc =>
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
        Vector3 vector(double x, double y, double z)=>new Vector3((double) x,(double) y,(double) z);

        [Fact]
        public void ShouldGetTheShortestLine()
        {
            var edge0 = new Edge3(vector(0,-1,-1), vector(0,1,-1));
            var edge1 = new Edge3(vector(-1,0,1), vector(1,0,1));

            var connect = edge0.ShortestEdgeJoining(edge1);

            connect.A.X.Should().BeApproximately(0, 1e-6);
            connect.A.Y.Should().BeApproximately(0, 1e-6);
            connect.A.Z.Should().BeApproximately(-1, 1e-6);

            connect.B.X.Should().BeApproximately(0, 1e-6);
            connect.B.Y.Should().BeApproximately(0, 1e-6);
            connect.B.Z.Should().BeApproximately(1, 1e-6);

        }
        
    }
}
