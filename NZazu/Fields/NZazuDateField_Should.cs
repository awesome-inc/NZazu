using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
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

            var datePicker = (DatePicker)sut.ValueControl;
            datePicker.Should().NotBeNull();
            datePicker.Text.Should().BeEmpty();
            datePicker.ToolTip.Should().Be(sut.Description);
        }

        [Test]
        //[SetUICulture("en-US")]
        public void Format_UIText_From_Value()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var sut = new NZazuDateField("test");
            const string dateFormat = "yyyy_MM_dd";
            sut.Settings = new Dictionary<string, string>() { { "Format", dateFormat } };
            var datePicker = (DatePicker)sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            datePicker.Text.Should().BeEmpty();

            var now = DateTime.Now.Date;
            sut.Value = now;
            var expected = now.ToString(dateFormat);
            sut.StringValue.Should().Be(expected);
            datePicker.Text.Should().Be(expected);

            sut.Value = null;
            sut.StringValue.Should().BeEmpty();
            datePicker.SelectedDate.Should().NotHaveValue();
        }

        [Test]
        public void Format_SelectedDate_From_Value()
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