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
            var sut = new NZazuLocationField(new FieldDefinition { Key = "test" }, type => null);

            sut.Should().NotBeNull();
            sut.ValueControl.Should().BeOfType<GeoLocationBox>();
            sut.Value.Should().BeNull();
        }
    }
}