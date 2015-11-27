using System.Linq;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    internal class NZazuOptionsField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuOptionsField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Type.Should().Be("option");
        }

        [Test(Description = "https://github.com/awesome-inc/NZazu/issues/68")]
        public void Create_ComboBox()
        {
            var sut = new NZazuOptionsField("test") { Description = "description"};

            sut.ContentProperty.Should().Be(ComboBox.TextProperty);
            var control = (ComboBox)sut.ValueControl;
            control.Should().NotBeNull();

            control.ToolTip.Should().Be(sut.Description);
        }

        [Test(Description = "https://github.com/awesome-inc/NZazu/issues/68")]
        public void Reflect_changing_Value_in_TextProperty()
        {
            var sut = new NZazuOptionsField("test")
            {
                Options = new[] { "1", "2", "3", "4", "5"}
            };

            var control = (ComboBox)sut.ValueControl;
            control.Items.Should().BeEquivalentTo(sut.Options);
            control.IsEditable.Should().BeFalse();

            sut.Value.Should().BeNull();
            control.SelectedItem.Should().BeNull();

            // value -> selected item
            var expected = sut.Options.First();
            sut.Value = expected;
            control.Text.Should().Be(expected);

            // selected item -> value
            expected = sut.Options.Last();
            control.SelectedItem = expected;
            sut.Value.Should().Be(expected);

            // custom values
            expected = "42";
            sut.Value = expected;
            sut.Value.Should().Be(expected);
            control.Text.Should().Be(expected);

            control.SelectedItem = expected;
            control.Text.Should().Be(expected);
        }

        [Test]
        public void Identify_Value_with_StringValue()
        {
            var sut = new NZazuOptionsField("test");

            sut.Value.Should().BeNull();
            sut.StringValue.Should().Be(sut.Value);

            sut.StringValue = "1";
            sut.Value.Should().Be(sut.StringValue);

            sut.Value = "2";
            sut.StringValue.Should().Be(sut.Value);
        }
    }
}