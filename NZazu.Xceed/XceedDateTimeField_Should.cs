using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixtureFor(typeof(XceedDateTimeField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedDateTimeField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new XceedDateTimeField("test", new FieldDefinition());

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Type.Should().Be("date");
        }

        [Test]
        public void Override_ContentProperty()
        {
            var sut = new XceedDateTimeField("date", new FieldDefinition());
            sut.ContentProperty.Should().Be(DateTimePicker.ValueProperty);
        }

        [Test]
        [STAThread]
        public void Use_Format_Settings()
        {
            var sut = new XceedDateTimeField("date", new FieldDefinition());
            const string dateFormat = "yyyy/MM/dd";
            sut.Settings.Add("Format", dateFormat);

            var control = (DateTimePicker)sut.ValueControl;
            control.Format.Should().Be(DateTimeFormat.Custom);
            control.FormatString.Should().Be(dateFormat);

            sut = new XceedDateTimeField("date", new FieldDefinition());
            control = (DateTimePicker)sut.ValueControl;
            control.Format.Should().Be(DateTimeFormat.FullDateTime);
            control.FormatString.Should().BeNull();
        }
    }
}