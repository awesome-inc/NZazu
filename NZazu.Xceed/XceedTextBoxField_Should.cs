using System;
using System.Threading;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixtureFor(typeof(XceedTextBoxField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedTextBoxField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new XceedTextBoxField(new FieldDefinition { Key = "test" });
            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();

            sut.Type.Should().Be("string");
        }

        [Test]
        [STAThread]
        public void Create_TextBox_with_ToolTip_Matching_Description()
        {
            var sut = new XceedTextBoxField("description", "hint", new FieldDefinition { Key = "textbox01" });

            sut.Description.Should().Be("description");
            sut.Hint.Should().Be("hint");

            var textBox = (WatermarkTextBox)sut.ValueControl;
            textBox.Should().NotBeNull();
            textBox.Text.Should().BeEmpty();
            textBox.ToolTip.Should().Be(sut.Description);
            textBox.Watermark.Should().Be(sut.Hint);
        }
    }
}