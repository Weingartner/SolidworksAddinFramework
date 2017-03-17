using System.Collections.Generic;
using FluentAssertions;
using Weingartner.WeinCad.Interfaces;
using Xunit;

namespace SolidworksAddinFramework.Spec
{
    public class ListExtensionsSpec
    {
        [Fact]
        public void RotateToHeadShouldWork()
        {
            var l = new List<int>() {1,2,3,4,5,6,7};

            l = l.RotateToHead(n => n == 4);

            l.Should().Equal(4, 5, 6, 7, 1, 2, 3);
        } 
    }
}