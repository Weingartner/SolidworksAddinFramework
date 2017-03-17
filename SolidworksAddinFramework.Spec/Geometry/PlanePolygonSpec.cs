using System;
using System.Linq;
using System.DoubleNumerics;
using FluentAssertions;
using SolidworksAddinFramework.Geometry;
using Weingartner.WeinCad.Interfaces;
using Xunit;

namespace SolidworksAddinFramework.Spec.Geometry
{
    public class PlanePolygonSpec
    {
        [Fact]
        public void OrientationShouldWork()
        {

            var points = Sequences.LinSpace(0, Math.PI*2, 100)
                .Select(t => new Vector3((double) Math.Cos(t), (double) Math.Sin(t), 0))
                .ToList();

            points.OrientationClosed(Vector3.UnitZ).Should().BeTrue();
            points.Reverse();
            points.OrientationClosed(Vector3.UnitZ).Should().BeFalse();



        } 
    }
}