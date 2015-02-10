using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuOptionsField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuOptionsField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Type.Should().Be("option");
        }

        [Test]
        public void Create_ComboBox()
        {
            var sut = new NZazuOptionsField("test") { Description = "description"};

            sut.ContentProperty.Should().Be(Selector.SelectedItemProperty);
            var control = (ComboBox)sut.ValueControl;
            control.Should().NotBeNull();

            control.ToolTip.Should().Be(sut.Description);
        }

        [Test]
        public void Bind_Value_to_SelectedItem()
        {
            var sut = new NZazuOptionsField("test")
            {
                Description = "description",
                Options = new[] { "1", "2", "3", "4", "5"}
            };

            var control = (ComboBox)sut.ValueControl;
            control.Items.Should().BeEquivalentTo(sut.Options);

            sut.Value.Should().BeNull();
            control.SelectedItem.Should().BeNull();

            var expected = sut.Options.First();
            sut.Value = expected;
            control.SelectedItem.Should().Be(expected);

            expected = sut.Options.Last();
            control.SelectedItem = expected;
            sut.Value.Should().Be(expected);
        }
    }
}