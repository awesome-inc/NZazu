using System.Threading;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuLocationField))]
    // ReSharper disable once InconsistentNaming
    internal class NZazuLocationField_Should
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Be_Creatable()
        {
            var sut = new NZazuLocationField(new FieldDefinition { Key = "test" });
            sut.Initialize(type => null); // we should create a func which return the isupportgeo implementation

            sut.Should().NotBeNull();
            sut.ValueControl.Should().BeOfType<GeoLocationBox>();
            sut.Type.Should().Be("location");
            sut.Value.Should().BeNull();
        }
    }
}