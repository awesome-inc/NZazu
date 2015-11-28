using System;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using Xceed.Wpf.Toolkit;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;

namespace NZazu.Xceed
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedRichTextField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new XceedRichTextField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        [STAThread]
        public void Override_ContentProperty_to_RichTextBox()
        {
            var field = new XceedRichTextField("key");
            field.ContentProperty.Should().Be(RichTextBox.TextProperty);

            var textBox = (RichTextBox) field.ValueControl;

            field.StringValue.Should().BeNullOrEmpty();
            textBox.Text.Should().BeNullOrEmpty();

            field.StringValue = "foobar";
            textBox.Text.Should().Be(field.StringValue);

            textBox.Text = "not foobar";
            field.StringValue.Should().Be(textBox.Text);
        }

        [Test]
        [STAThread]
        public void Set_Vertical_Scrollbar()
        {
            var field = new XceedRichTextField("key");
            var textBox = (RichTextBox)field.ValueControl;
            textBox.VerticalScrollBarVisibility.Should().Be(ScrollBarVisibility.Auto);
        }

        [Test]
        [STAThread]
        public void Respect_Height_Setting()
        {
            var field = new XceedRichTextField("key");
            var expectedHeight = 2*XceedRichTextField.DefaultHeight;
            field.Settings.Add("Height", expectedHeight.ToString(CultureInfo.InvariantCulture));
            
            var textBox = (RichTextBox)field.ValueControl;
            textBox.MinHeight.Should().Be(expectedHeight);
            textBox.MaxHeight.Should().Be(expectedHeight);

            field = new XceedRichTextField("key");
            expectedHeight = XceedRichTextField.DefaultHeight;
            field.Settings.Add("Height", "not a number");

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
            var field = new XceedRichTextField("key") {Settings = {["Format"] = format}};

            var textBox = (RichTextBox)field.ValueControl;
            textBox.TextFormatter.Should().BeOfType(formatterType);
        }

        [Test]
        [STAThread]
        public void Add_optional_RichTextFormatBar()
        {
            var field = new XceedRichTextField("key");
            var textBox = (RichTextBox)field.ValueControl;
            var formatBar = RichTextBoxFormatBarManager.GetFormatBar(textBox);
            formatBar.Should().BeNull();

            field = new XceedRichTextField("key") {Settings = {["ShowFormatBar"] = true.ToString()}};

            textBox = (RichTextBox)field.ValueControl;
            formatBar = RichTextBoxFormatBarManager.GetFormatBar(textBox);
            formatBar.Should().NotBeNull();
        }
    }
}