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

            r[0].Value.Should().Equal(2, 4, 6, 8);
            r[1].Value.Should().Equal(1, 3, 7);
            r[2].Value.Should().Equal(2);
            r[3].Value.Should().Equal(9, 7, 11);
            r[4].Value.Should().Equal(4, 8, 100);


            new int[] {}.BufferTillChanged(v => v).ToList().Count.Should().Be(0);
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
    }

}