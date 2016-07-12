using System;
using System.Linq;
using FluentAssertions;
using LanguageExt;
using Xunit;

namespace SolidworksAddinFramework.Spec
{
    public class GetTrimCurves2DataReaderSpec
    {
        [Fact]
        public void CanReadData()
        {
            var data = new[] {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0};
            var reader = new GetTrimCurves2DataReader(data);

            reader.Read(2).ShouldBeEquivalentTo(new [] { 1.0,2.0});
            reader.Read(3).ShouldBeEquivalentTo(new [] { 3.0,4.0,5.0});

        }

        [Fact]
        public void DoubleToShortShouldWork()
        {
            var expected = new short[] {2, 3, 5, 7};
            var bytes = expected.SelectMany(BitConverter.GetBytes).ToArray();
            var d = BitConverter.ToDouble(bytes,0);
            var data = d.DoubleToShort();

            data.Item1.Should().Be(2);
            data.Item2.Should().Be(3);
            data.Item3.Should().Be(5);
            data.Item4.Should().Be(7);
        }
    }
}