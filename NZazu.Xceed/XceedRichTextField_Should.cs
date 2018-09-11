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
using Xceed.Wpf.Toolkit;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;

namespace NZazu.Xceed
{
    [TestFixtureFor(typeof(XceedRichTextField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedRichTextField_Should
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
            var sut = new XceedRichTextField(new FieldDefinition { Key = "key" }, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        [STAThread]
        public void Override_ContentProperty_to_RichTextBox()
        {
            var field = new XceedRichTextField(new FieldDefinition { Key = "key" }, ServiceLocator);
            field.ContentProperty.Should().Be(RichTextBox.TextProperty);

            var textBox = (RichTextBox)field.ValueControl;

            field.GetStringValue().Should().BeNullOrEmpty();
            textBox.Text.Should().BeNullOrEmpty();

            field.SetStringValue("foobar");
            textBox.Text.Should().Be(field.GetStringValue());

            textBox.Text = "not foobar";
            field.GetStringValue().Should().Be(textBox.Text);
        }

        [Test]
        [STAThread]
        public void Set_Vertical_Scrollbar()
        {
            var field = new XceedRichTextField(new FieldDefinition { Key = "key" }, ServiceLocator);
            var textBox = (RichTextBox)field.ValueControl;
            textBox.VerticalScrollBarVisibility.Should().Be(ScrollBarVisibility.Auto);
        }

        [Test]
        [STAThread]
        public void Respect_Height_Setting()
        {
            var field = new XceedRichTextField(new FieldDefinition { Key = "key" }, ServiceLocator);
            var expectedHeight = 2 * XceedRichTextField.DefaultHeight;
            field.Definition.Settings.Add("Height", expectedHeight.ToString(CultureInfo.InvariantCulture));

            var textBox = (RichTextBox)field.ValueControl;
            textBox.MinHeight.Should().Be(expectedHeight);
            textBox.MaxHeight.Should().Be(expectedHeight);

            field = new XceedRichTextField(new FieldDefinition { Key = "key" }, ServiceLocator);
            expectedHeight = XceedRichTextField.DefaultHeight;
            field.Definition.Settings.Add("Height", "not a number");

            textBox = (RichTextBox)field.ValueControl;
            textBox.MinHeight.Should().Be(expectedHeight);
        }

        [Test]
        [TestCase("rtf", typeof(RtfFormatter))]
        [TestCase("plain", typeof(PlainTextFormatter))]
        [TestCase("xaml", typeof(XamlFormatter))]
        [TestCase(null, typeof(RtfFormatter))]
        [TestCase("foobar", typeof(RtfFormatter))]
        [STAThread]
        public void Respect_Format_Setting(string format, Type formatterType)
        {
            var field = new XceedRichTextField(new FieldDefinition { Key = "key", Settings = { ["Format"] = format } }, ServiceLocator);

            var textBox = (RichTextBox)field.ValueControl;
            textBox.TextFormatter.Should().BeOfType(formatterType);
        }

        [Test]
        [STAThread]
        public void Add_optional_RichTextFormatBar()
        {
            var field = new XceedRichTextField(new FieldDefinition { Key = "key" }, ServiceLocator);
            var textBox = (RichTextBox)field.ValueControl;
            var formatBar = RichTextBoxFormatBarManager.GetFormatBar(textBox);
            formatBar.Should().BeNull();

            field = new XceedRichTextField(new FieldDefinition { Key = "key", Settings = { ["ShowFormatBar"] = true.ToString() } }, ServiceLocator);

            textBox = (RichTextBox)field.ValueControl;
            formatBar = RichTextBoxFormatBarManager.GetFormatBar(textBox);
            formatBar.Should().NotBeNull();
        }
    }
}