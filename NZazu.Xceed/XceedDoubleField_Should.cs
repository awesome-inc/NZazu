using System;
using FluentAssertions;
using NUnit.Framework;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class XceedDoubleField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new XceedDoubleField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuField>();
            sut.Type.Should().Be("double");
        }

        [Test]
        public void Use_DoubleUpdown()
        {
            var sut = new XceedDoubleField("test");
            sut.ContentProperty.Should().Be(DoubleUpDown.ValueProperty);

            var control = (DoubleUpDown) sut.ValueControl;
            control.Should().NotBeNull();
        }

        [Test]
        public void Format_ControlValue_From_StringValue()
        {
            var sut = new XceedDoubleField("test");
            var control = (DoubleUpDown)sut.ValueControl;

            sut.StringValue.Should().BeNullOrEmpty();
            control.Value.Should().NotHaveValue();

            sut.StringValue = "1.4";
            control.Value.Should().Be(1.4);

            sut.StringValue = String.Empty;
            control.Value.Should().NotHaveValue();
        }

        [Test]
        public void Format_StringValue_From_ControlValue()
        {
            var sut = new XceedDoubleField("test");
            var control = (DoubleUpDown)sut.ValueControl;

            control.Value = 1.4;
            sut.StringValue.Should().Be("1.4");

            control.Value = null;
            sut.IsValid().Should().BeTrue();
            sut.StringValue.Should().Be("");

            control.Text = null;
            sut.IsValid().Should().BeTrue();
            sut.StringValue.Should().Be(String.Empty);
        }

    }
}