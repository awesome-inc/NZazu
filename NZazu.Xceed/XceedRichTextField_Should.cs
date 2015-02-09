using System.Globalization;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;

namespace NZazu.Xceed
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class XceedRichTextField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new XceedRichTextField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
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
        public void Set_Vertical_Scrollbar()
        {
            var field = new XceedRichTextField("key");
            var textBox = (RichTextBox)field.ValueControl;
            textBox.VerticalScrollBarVisibility.Should().Be(ScrollBarVisibility.Auto);
        }

        [Test]
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
    }
}