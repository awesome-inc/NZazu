using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Adapter
{
    [TestFixtureFor(typeof(SupportGeoLocationBox))]
    // ReSharper disable once InconsistentNaming
    internal class SupportGeoLocationBox_Should
    {
        [Test]
        public void Be_Creatble()
        {
            var sut = new SupportGeoLocationBox();
            sut.Should().NotBeNull();

            var data = new NZazuCoordinate {Lat = 23.4, Lon = 56.7};
            data.Should().BeEquivalentTo(sut.Parse(sut.ToString(data)));
        }
    }
}