using System;
using System.DoubleNumerics;
using FluentAssertions;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.OpenGl;
using Weingartner.WeinCad.Interfaces;
using Xunit;

namespace SolidworksAddinFramework.Spec.opengl
{
    public class Vector3ExtensionsSpec
    {
        [Fact]
        public void AnyOrthogonalShouldWork()
        {

            var r = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var v = new Vector3((double) r.NextDouble(), (double) r.NextDouble(), (double) r.NextDouble());
                var o = v.Orthogonal();
                v.Dot(o).Should().BeApproximately(0, 1e-9);

            }
            
        }

        [Fact]
        public void OrientationShouldWork()
        {
            Vector3.UnitX.Orientation( Vector3.UnitY, Vector3.UnitZ).Should().Be(1);
            Vector3.UnitY.Orientation( Vector3.UnitX, Vector3.UnitZ).Should().Be(-1);
        }
    }
}