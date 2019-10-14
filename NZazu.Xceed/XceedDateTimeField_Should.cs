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
    [TestFixtureFor(typeof(XceedDateTimeField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedDateTimeField_Should
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
            var sut = new XceedDateTimeField(new FieldDefinition {Key = "key"}, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        public void Override_ContentProperty()
        {
            var sut = new XceedDateTimeField(new FieldDefinition {Key = "date"}, ServiceLocator);
            sut.ContentProperty.Should().Be(DateTimePickerWithUpdate.ValueProperty);
        }

        [Test]
        [STAThread]
        public void Use_Format_Settings()
        {
            var sut = new XceedDateTimeField(new FieldDefinition {Key = "date"}, ServiceLocator);
            const string dateFormat = "yyyy/MM/dd";
            sut.Definition.Settings.Add("Format", dateFormat);

            var control = (DateTimePickerWithUpdate) sut.ValueControl;
            control.Format.Should().Be(DateTimeFormat.Custom);
            control.FormatString.Should().Be(dateFormat);

            sut = new XceedDateTimeField(new FieldDefinition {Key = "date"}, ServiceLocator);
            control = (DateTimePickerWithUpdate) sut.ValueControl;
            control.Format.Should().Be(DateTimeFormat.FullDateTime);
            control.FormatString.Should().BeNull();
        }
    }
}