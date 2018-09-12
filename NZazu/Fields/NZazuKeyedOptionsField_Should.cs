using System;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuKeyedOptionsField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuKeyedOptionsField_Should
    {
        private object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuKeyedOptionsField(new FieldDefinition { Key = "test" }, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test(Description = "https://github.com/awesome-inc/NZazu/issues/68")]
        [STAThread]
        public void Create_ComboBox()
        {
            var definition = new FieldDefinition { Key = "test", Description = "description" };
            var sut = new NZazuKeyedOptionsField(definition, ServiceLocator);

            sut.ContentProperty.Should().Be(ComboBox.TextProperty);
            var control = (ComboBox)sut.ValueControl;
            control.Should().NotBeNull();

            control.ToolTip.Should().Be(sut.Definition.Description);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Identify_Value_with_StringValue()
        {
            var sut = new NZazuKeyedOptionsField(new FieldDefinition { Key = "test", Description = "description" }, ServiceLocator);

            sut.Value.Should().BeNull();
            sut.GetValue().Should().Be(sut.Value);

            sut.SetValue( "1");
            sut.Value.Should().Be(sut.GetValue());

            sut.Value = "2";
            sut.GetValue().Should().Be(sut.Value);
        }
    }
}