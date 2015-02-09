using System;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuTextField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuTextField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        public void Create_TextBox_with_ToolTip_Matching_Description()
        {
            var sut = new NZazuTextField("test")
            {
                Hint = "superhero",
                Description = "check this if you are a registered superhero"
            };

            var label = (TextBox)sut.ValueControl;
            label.Should().NotBeNull();
            label.Text.Should().BeEmpty();
            label.ToolTip.Should().Be(sut.Description);
        }

        [Test]
        public void Create_ValueControl_Even_If_Empty_Hint()
        {
            var sut = new NZazuTextField("test");

            var label = (TextBox)sut.ValueControl;
            label.Should().NotBeNull();
            label.Text.Should().BeEmpty();
        }

        [Test]
        public void Get_Set_Value_should_propagate_to_ValueControl_Without_LostFocus()
        {
            var sut = new NZazuTextField("test");
            sut.StringValue.Should().BeNull();

            sut.StringValue = "test";
            sut.StringValue.Should().Be("test");
            ((TextBox)sut.ValueControl).Text.Should().Be("test");

            ((TextBox)sut.ValueControl).Text = String.Empty;
            sut.StringValue.Should().BeEmpty();
        }
    }
}