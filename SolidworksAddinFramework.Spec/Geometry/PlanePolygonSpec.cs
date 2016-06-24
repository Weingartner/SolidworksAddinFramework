using System;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using SolidworksAddinFramework.Geometry;
using Weingartner.Numerics;
using Xunit;

namespace SolidworksAddinFramework.Spec.Geometry
{
    public class PlanePolygonSpec
    {
        [Fact]
        public void OrientationShouldWork()
        {

            var points = Sequences.LinSpace(0, Math.PI*2, 100)
                .Select(t => new Vector3((float) Math.Cos(t), (float) Math.Sin(t), 0))
                .ToList();

            points.OrientationClosed(Vector3.UnitZ).Should().BeTrue();
            points.Reverse();
            points.OrientationClosed(Vector3.UnitZ).Should().BeFalse();



        } 
    }
}