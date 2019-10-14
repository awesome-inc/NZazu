using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Adapter;
using NZazu.Contracts.Suggest;
using NZazu.Extensions;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuLocationField))]
    // ReSharper disable once InconsistentNaming
    internal class NZazuLocationField_Should
    {
        [ExcludeFromCodeCoverage]
        private object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            if (type == typeof(ISupportGeoLocationBox)) return new SupportGeoLocationBox();
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Be_Creatable()
        {
            var sut = new NZazuLocationField(new FieldDefinition {Key = "test"}, ServiceLocator);

            sut.Should().NotBeNull();
            sut.ValueControl.Should().BeOfType<GeoLocationBox>();
            sut.Value.Should().BeNull();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Get_And_Set_Values()
        {
            var sut = new NZazuLocationField(new FieldDefinition {Key = "test"}, ServiceLocator);
            var box = (GeoLocationBox) sut.ValueControl;

            sut.SetValue("23 34");
            sut.GetValue().Should().Be("23 34");
            box.Value.Lat.Should().Be(23);
            box.Value.Lon.Should().Be(34);

            box.Value = new NZazuCoordinate {Lat = 11, Lon = 22};
            sut.GetValue().Should().Be("11 22");
        }
    }
}