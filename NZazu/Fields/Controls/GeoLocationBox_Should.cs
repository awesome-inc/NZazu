using System.Threading;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts.Adapter;
using NZazu.Contracts.Suggest;

namespace NZazu.Fields.Controls
{
    [TestFixtureFor(typeof(GeoLocationBox))]
    // ReSharper disable once InconsistentNaming
    internal class GeoLocationBox_Should
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Be_Creatable()
        {
            var sut = new GeoLocationBox();
            // we need to set the formatter first (so we don't get an exception if the default formatter is wrong
            var formatter = Substitute.For<ISupportGeoLocationBox>();

            sut.GeoLocationSupport.Should().BeNull();
            sut.GeoLocationSupport = formatter;
            sut.GeoLocationSupport.Should().Be(formatter);

            sut.Value.Should().Be(null);
            sut.Value = new NZazuCoordinate {Lat = 53.1, Lon = 7.2};
            sut.Value.Lat.Should().Be(53.1);
            sut.Value.Lon.Should().Be(7.2);

            sut.IsReadOnly.Should().BeFalse();
            sut.IsReadOnly = true;
            sut.IsReadOnly.Should().BeTrue();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Foo()
        {
            var sut = new GeoLocationBox();
            var formatter = Substitute.For<ISupportGeoLocationBox>();
            sut.GeoLocationSupport = formatter;

            sut.OpenInGeoAppClick(null, null);
            sut.SetToCurrentLocationClick(null, null);
        }
    }
}