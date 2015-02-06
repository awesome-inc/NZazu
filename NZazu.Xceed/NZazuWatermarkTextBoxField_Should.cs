using FluentAssertions;
using NUnit.Framework;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixture, RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuWatermarkTextBoxField_Should
    {
        [Test]
        public void Set_Watermark_according_to_Hint()
        {
            const string hint = "Enter name";
            var sut = new NZazuWatermarkTextBoxField("test") {}; // {Hint = hint}};

            var control = (WatermarkTextBox) sut.ValueControl;
            control.Should().NotBeNull();
            control.Should().BeOfType<WatermarkTextBox>();

            control.Watermark.Should().Be(hint);
        }
    }
}