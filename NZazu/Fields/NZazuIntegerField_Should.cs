using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof (NZazuIntegerField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuIntegerField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuIntegerField(new FieldDefinition {Key="test"});

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Type.Should().Be("int");
        }

        [Test]
        [STAThread]
        public void Create_Control_With_ToolTip_Matching_Description()
        {
            var sut = new NZazuIntegerField(new FieldDefinition {Key="test"})
            {
                Hint = "superhero",
                Description = "check this if you are a registered superhero"
            };

            var textBox = (TextBox)sut.ValueControl;
            textBox.Should().NotBeNull();
            textBox.Text.Should().BeEmpty();
            textBox.ToolTip.Should().Be(sut.Description);
        }

        [Test]
        [STAThread]
        public void Format_TextBox_From_Value()
        {
            var sut = new NZazuIntegerField(new FieldDefinition {Key="test"});
            var textBox = (TextBox)sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            textBox.Text.Should().BeEmpty();

            sut.Value = 42;
            textBox.Text.Should().Be("42");

            sut.Value = -23;
            textBox.Text.Should().Be("-23");

            sut.Value = null;
            textBox.Text.Should().Be(string.Empty);
        }

        [Test]
        [STAThread]
        public void Format_Value_From_TextBox()
        {
            var sut = new NZazuIntegerField(new FieldDefinition {Key="test"});
            var textBox = (TextBox)sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            textBox.Text.Should().BeEmpty();

            textBox.Text = "7";
            sut.Value.Should().Be(7);

            textBox.Text = "-12";
            sut.Value.Should().Be(-12);

            textBox.Text = "foo bar";
            sut.IsValid().Should().BeFalse();
            sut.Value.Should().Be(-12, because: "WPF binding cannot sync value");

            textBox.Text = "";
            sut.IsValid().Should().BeTrue();
            sut.Value.Should().NotHaveValue();
        }

        [Test]
        [STAThread]
        public void Format_TextBox_From_StringValue()
        {
            var sut = new NZazuIntegerField(new FieldDefinition {Key="test"});
            var textBox = (TextBox)sut.ValueControl;

            sut.StringValue.Should().BeNullOrEmpty();
            textBox.Text.Should().BeEmpty();

            sut.StringValue = "42";
            textBox.Text.Should().Be("42");

            sut.StringValue = "-23";
            textBox.Text.Should().Be("-23");

            sut.StringValue = null;
            textBox.Text.Should().Be(string.Empty);
        }

        [Test]
        [STAThread]
        public void Format_StringValue_From_TextBox()
        {
            var sut = new NZazuIntegerField(new FieldDefinition {Key="test"});
            var textBox = (TextBox)sut.ValueControl;

            textBox.Text = "7";
            sut.StringValue.Should().Be("7");

            textBox.Text = "-12";
            sut.StringValue.Should().Be("-12");

            textBox.Text = "foo bar";
            sut.IsValid().Should().BeFalse();
            sut.StringValue.Should().Be("-12", because: "WPF binding cannot sync value");

            textBox.Text = "";
            sut.IsValid().Should().BeTrue();
            sut.StringValue.Should().Be("");

            // ReSharper disable once AssignNullToNotNullAttribute
            textBox.Text = null;
            sut.IsValid().Should().BeTrue();
            sut.StringValue.Should().Be(string.Empty);
        }
    }
}