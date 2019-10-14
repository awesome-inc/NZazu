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
    [TestFixtureFor(typeof(XceedTextBoxField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedTextBoxField_Should
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
            var sut = new XceedTextBoxField(new FieldDefinition {Key = "test"}, ServiceLocator);
            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        [STAThread]
        public void Create_TextBox_with_ToolTip_Matching_Description()
        {
            var sut = new XceedTextBoxField(new FieldDefinition
            {
                Key = "textbox01",
                Description = "description",
                Hint = "hint"
            }, ServiceLocator);

            sut.Definition.Description.Should().Be("description");
            sut.Definition.Hint.Should().Be("hint");

            var textBox = (WatermarkTextBox) sut.ValueControl;
            textBox.Should().NotBeNull();
            textBox.Text.Should().BeEmpty();
            textBox.ToolTip.Should().Be(sut.Definition.Description);
            textBox.Watermark.Should().Be(sut.Definition.Hint);
        }
    }
}