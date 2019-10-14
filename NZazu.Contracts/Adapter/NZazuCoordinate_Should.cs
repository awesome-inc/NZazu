using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Adapter
{
    [TestFixtureFor(typeof(NZazuCoordinate))]
    // ReSharper disable once InconsistentNaming
    internal class NZazuCoordinate_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuCoordinate {Lat = 23, Lon = -10};

            sut.Should().NotBeNull();
            sut.Lat.Should().Be(23);
            sut.Lon.Should().Be(-10);
        }

        [Test]
        [TestCase("23,1 -34,2", 23.1, -34.2)]
        public void Parse_German(string toParse, double lat, double lon)
        {
            var sut = NZazuCoordinate.Parse(toParse);

            sut.Should().NotBeNull();
            sut.Lat.Should().Be(lat);
            sut.Lon.Should().Be(lon);
        }

        [Test]
        [TestCase("23.1 34.2", 23.1, 34.2)]
        [TestCase("-23.99 34.0001", -23.99, 34.0001)]
        [TestCase("23.123123 -34.456456", 23.123123, -34.456456)]
        [TestCase("23 34", 23, 34)]
        [TestCase("-23 34", -23, 34)]
        [TestCase("23 -34", 23, -34)]
        public void Parse_LatLonDecimal(string toParse, double lat, double lon)
        {
            var sut = NZazuCoordinate.Parse(toParse);

            sut.Should().NotBeNull();
            sut.Lat.Should().Be(lat);
            sut.Lon.Should().Be(lon);

            // at least it should be symetric
            var str = sut.ToString();
            str.Should().Be(toParse);

            // and parse its own dog-food
            sut = NZazuCoordinate.Parse(str);
            sut.Should().NotBeNull();
            sut.Lat.Should().Be(lat);
            sut.Lon.Should().Be(lon);
        }
    }
}