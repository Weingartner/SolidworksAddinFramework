using System.Linq;
using FluentAssertions;
using Weingartner.WeinCad.Interfaces;
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

            r[0].Value.Should().Equal(2, 4, 6, 8);
            r[1].Value.Should().Equal(1, 3, 7);
            r[2].Value.Should().Equal(2);
            r[3].Value.Should().Equal(9, 7, 11);
            r[4].Value.Should().Equal(4, 8, 100);


            new int[] {}.BufferTillChanged(v => v).ToList().Count.Should().Be(0);
        }

        [Fact]
        public void TransposeShouldWork()
        {
            var t = new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 } };

            var r = t.Transpose().Select(v => v.ToList()).ToList();

            r[0].Should().Equal(1, 4, 7);
            r[1].Should().Equal(2, 5, 8);
            r[2].Should().Equal(3, 6, 9);

            r.Count.Should().Be(3);

        }

        [Fact]
        public void GroupAdjacentShouldWork()
        {
            var e = new[] {0, 0, 1, 1, 2, 3, 4, 2, 2, 2, 7, 9};
            var result = e
                .GroupAdjacent(v => v)
                .ToList();

            result.Count.Should().Be(8);
            result[0].Should().Equal(0, 0);
            result[1].Should().Equal(1, 1);
            result[2].Should().Equal(2);
            result[3].Should().Equal(3);
            result[4].Should().Equal(4);
            result[5].Should().Equal(2, 2, 2);
            result[6].Should().Equal(7);
            result[7].Should().Equal(9);
        }
    }

}