using System;
using FluentAssertions;
using NUnit.Framework;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixture, RequiresSTA]
    // ReSharper disable InconsistentNaming
    class XceedDateTimeField_Should
    {
        [Test]
        public void Override_ContentProperty()
        {
            var sut = new XceedDateTimeField("date");
            sut.ContentProperty.Should().Be(DateTimePicker.ValueProperty);
        }

        [Test]
        public void Use_Format_Settings()
        {
            var sut = new XceedDateTimeField("date");
            const string dateFormat = "yyyy/MM/dd";
            sut.Settings.Add("Format", dateFormat);

            var control = (DateTimePicker) sut.ValueControl;
            control.Format.Should().Be(DateTimeFormat.Custom);
            control.FormatString.Should().Be(dateFormat);

            sut = new XceedDateTimeField("date");
            control = (DateTimePicker)sut.ValueControl;
            control.Format.Should().Be(DateTimeFormat.FullDateTime);
            control.FormatString.Should().BeNull();
        }
    }
}