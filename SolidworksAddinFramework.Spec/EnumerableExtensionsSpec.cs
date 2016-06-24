using System.Linq;
using FluentAssertions;
using Xunit;

namespace SolidworksAddinFramework.Spec
{
    public class EnumerableExtensionsSpec
    {
        [Fact]
        public void BufferWhileNotChangedShouldWork()
        {
            var list = new[] {2, 4, 6, 8, 1, 3, 7, 2, 9, 7, 11, 4, 8, 100};

            var r = list.BufferTillChanged(v => v % 2).ToList();

            r[0].Should().Equal(2, 4, 6, 8);
            r[1].Should().Equal(1, 3, 7);
            r[2].Should().Equal(2);
            r[3].Should().Equal(9, 7, 11);
            r[4].Should().Equal(4, 8, 100);


            new int[] {}.BufferTillChanged(v => v).ToList().Count.Should().Be(0);

        }
        
    }
}