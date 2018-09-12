using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixtureFor(typeof(XceedDoubleField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedDoubleField_Should
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
            var sut = new XceedDoubleField(new FieldDefinition { Key = "key" }, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        [STAThread]
        public void Use_DoubleUpdown()
        {
            var sut = new XceedDoubleField(new FieldDefinition { Key = "key" }, ServiceLocator);
            sut.ContentProperty.Should().Be(DoubleUpDown.ValueProperty);

            var control = (DoubleUpDown) sut.ValueControl;
            control.Should().NotBeNull();
        }

        [Test]
        [STAThread]
        public void Format_ControlValue_From_StringValue()
        {
            var sut = new XceedDoubleField(new FieldDefinition { Key = "key" }, ServiceLocator);
            var control = (DoubleUpDown)sut.ValueControl;

            sut.GetValue().Should().BeNullOrEmpty();
            control.Value.Should().NotHaveValue();

            sut.SetValue("1.4");
            control.Value.Should().Be(1.4);

            sut.SetValue(string.Empty);
            control.Value.Should().NotHaveValue();
        }

        [Test]
        [STAThread]
        public void Format_StringValue_From_ControlValue()
        {
            var sut = new XceedDoubleField(new FieldDefinition { Key = "key" }, ServiceLocator);
            var control = (DoubleUpDown)sut.ValueControl;

            control.Value = 1.4;
            sut.GetValue().Should().Be("1.4");

            control.Value = null;
            sut.IsValid().Should().BeTrue();
            sut.GetValue().Should().Be("");

            control.Text = null;
            sut.IsValid().Should().BeTrue();
            sut.GetValue().Should().Be(string.Empty);
        }

        [Test]
        [STAThread]
        public void Not_Set_null_format_string()
        {
            var sut = new XceedDoubleField(new FieldDefinition { Key = "key" }, ServiceLocator);
            sut.Definition.Settings.Add("Format", null);

            var control = (DoubleUpDown)sut.ValueControl;
            control.FormatString.Should().NotBeNull();

            const string format = "#.00";
            sut = new XceedDoubleField(new FieldDefinition { Key = "key" }, ServiceLocator);
            sut.Definition.Settings.Add("Format", format);

            control = (DoubleUpDown)sut.ValueControl;
            control.FormatString.Should().Be(format);
        }
    }
}