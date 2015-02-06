using System;
using System.Globalization;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuDateField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuDateField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuField>();
            sut.Type.Should().Be("date");
        }

        [Test]
        public void Create_Control_With_ToolTip_Matching_Description()
        {
            var sut = new NZazuDateField("test")
            {
                Hint = "superhero",
                Description = "check this if you are a registered superhero"
            };

            var textBox = (DatePicker)sut.ValueControl;
            textBox.Should().NotBeNull();
            textBox.Text.Should().BeEmpty();
            textBox.ToolTip.Should().Be(sut.Description);
        }

        [Test]
        public void Format_TextBox_From_Value()
        {
            var sut = new NZazuDateField("test");
            var datePicker = (DatePicker)sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            datePicker.Text.Should().BeEmpty();

            var now = DateTime.Now.Date;
            sut.Value = now;
            datePicker.SelectedDate.Should().Be(now);

            sut.Value = null;
            datePicker.SelectedDate.Should().NotHaveValue();
        }

        [Test]
        public void Format_Value_From_TextBox()
        {
            var sut = new NZazuDateField("test");
            var datePicker = (DatePicker)sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            datePicker.Text.Should().BeEmpty();

            var now = DateTime.Now.Date;
            datePicker.SelectedDate = now;
            sut.Value.Should().Be(now);

            datePicker.SelectedDate = null;
            sut.IsValid().Should().BeTrue();
            sut.Value.Should().NotHaveValue();
        }

        [Test]
        public void Format_TextBox_From_StringValue()
        {
            var sut = new NZazuDateField("test");
            var datePicker = (DatePicker)sut.ValueControl;

            sut.StringValue.Should().BeNullOrEmpty();
            datePicker.Text.Should().BeEmpty();

            var now = DateTime.Now.Date;
            sut.StringValue = now.ToString(CultureInfo.InvariantCulture);
            datePicker.SelectedDate.Should().Be(now);

            sut.StringValue = "";
            datePicker.SelectedDate.Should().NotHaveValue();
        }

        [Test]
        public void Format_StringValue_From_TextBox()
        {
            var sut = new NZazuDateField("test");
            var datePicker = (DatePicker)sut.ValueControl;

            var now = DateTime.Now.Date;
            datePicker.SelectedDate = now;
            sut.StringValue.Should().Be(now.ToString(CultureInfo.InvariantCulture));

            datePicker.SelectedDate = null;
            sut.IsValid().Should().BeTrue();
            sut.StringValue.Should().Be("");

            datePicker.Text = null;
            sut.IsValid().Should().BeTrue();
            sut.StringValue.Should().Be(String.Empty);
        }
    }
}