using System;
using System.Numerics;
using FluentAssertions;
using SolidworksAddinFramework.OpenGl;
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
                var v = new Vector3((float) r.NextDouble(), (float) r.NextDouble(), (float) r.NextDouble());
                var o = v.Orthogonal();
                v.Dot(o).Should().BeApproximately(0, 1e-9);

            }
            
        } 
    }
}