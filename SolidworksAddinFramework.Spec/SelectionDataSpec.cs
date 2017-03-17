using FluentAssertions;
using Xunit;

namespace SolidworksAddinFramework.Spec
{
    public class SelectionDataSpec
    {
        [Fact]
        public void EqualityShouldWork()
        {
            var d1 = new SelectionData(new[] { new SelectionData.ObjectId(new byte[] { 1, 2, 3 }) }, 5);
            var d2 = new SelectionData(new[] { new SelectionData.ObjectId(new byte[] { 1, 2, 3 }) }, 5);
            d1.Equals(d2).Should().BeTrue();
        }
    }
}
