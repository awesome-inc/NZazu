using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof (NZazuBoolField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuBoolField_Should
    {
        [Test]
        [STAThread]
        public void Be_Creatable()
        {
            var sut = new NZazuBoolField("test", new FieldDefinition());

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();

            sut.Type.Should().Be("bool");
        }

        [Test]
        [STAThread]
        public void Not_Create_Empty_Label()
        {
            var sut = new NZazuBoolField("test", new FieldDefinition());
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        [STAThread]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuBoolField("test", new FieldDefinition())
            {
                Prompt = "superhero"
            };

            var label = (Label)sut.LabelControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Prompt);
        }

        [Test]
        [STAThread]
        public void Create_ValueControl_Matching_Description()
        {
            var sut = new NZazuBoolField("test", new FieldDefinition())
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
        [STAThread]
        public void Create_ValueControl_Even_If_Empty_Hint()
        {
            var sut = new NZazuBoolField("test", new FieldDefinition());

            var label = (CheckBox)sut.ValueControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Hint);
        }

        [Test]
        [STAThread]
        public void Get_Set_Value_should_propagate_to_ValueControl()
        {
            var sut = new NZazuBoolField("test", new FieldDefinition());
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

        [Test]
        [STAThread]
        public void Support_ThreeState_by_default()
        {
            var sut = new NZazuBoolField("test", new FieldDefinition());

            ((CheckBox)sut.ValueControl).IsThreeState.Should().BeTrue();
        }

        [Test]
        [STAThread]
        public void Center_checkboxes_vertically()
        {
            var sut = new NZazuBoolField("test", new FieldDefinition());
            var checkBox = (CheckBox)sut.ValueControl;
            checkBox.VerticalContentAlignment.Should().Be(VerticalAlignment.Center);
        }
    }
}