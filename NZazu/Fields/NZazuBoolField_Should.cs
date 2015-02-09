using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuBoolField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuBoolField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        public void Not_Create_Empty_Label()
        {
            var sut = new NZazuBoolField("test");
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuBoolField("test")
            {
                Prompt = "superhero"
            };

            var label = (Label)sut.LabelControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Prompt);
        }

        [Test]
        public void Create_ValueControl_Matching_Description()
        {
            var sut = new NZazuBoolField("test")
            {
                Hint = "superhero",
                Description = "check this if you are a registered superhero"
            };

            var label = (CheckBox)sut.ValueControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Hint);
            label.ToolTip.Should().Be(sut.Description);
        }

        [Test]
        public void Create_ValueControl_Even_If_Empty_Hint()
        {
            var sut = new NZazuBoolField("test");

            var label = (CheckBox)sut.ValueControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Hint);
        }

        [Test]
        public void Get_Set_Value_should_propagate_to_ValueControl()
        {
            var sut = new NZazuBoolField("test");
            sut.StringValue.Should().BeEmpty();
            var checkBox = (CheckBox)sut.ValueControl;

            // set
            sut.Value = true;
            checkBox.IsChecked.Should().Be(true);
            sut.StringValue.Should().Be("True");

            sut.StringValue = "false";
            checkBox.IsChecked.Should().Be(false);
            sut.StringValue.Should().Be("False");

            sut.StringValue = "foobar";
            checkBox.IsChecked.Should().NotHaveValue();

            // get
            checkBox.IsChecked = true;
            sut.StringValue.Should().Be("True");

            checkBox.IsChecked = false;
            sut.StringValue.Should().Be("False");

            checkBox.IsChecked = null;
            sut.StringValue.Should().BeEmpty();
        }
    }
}