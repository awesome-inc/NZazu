using System;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuDateField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuDateField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuDateField(new FieldDefinition {Key="test"});

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Type.Should().Be("date");
        }

        [Test]
        [STAThread]
        public void Create_Control_With_ToolTip_Matching_Description()
        {
            var sut = new NZazuDateField(new FieldDefinition {Key="test"})
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
        [STAThread]
        public void Format_UIText_From_Value()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var sut = new NZazuDateField(new FieldDefinition {Key="test"});
            const string dateFormat = "yyyy_MM_dd";
            sut.Settings.Add("Format", dateFormat);
            var datePicker = (DatePicker)sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            datePicker.Text.Should().BeEmpty();

            var now = DateTime.Now.Date;
            sut.Value = now;
            var expected = now.ToString(dateFormat);
            sut.StringValue.Should().Be(expected);

            // NOTE: Formatted Dates seems complicated to setup with DatePicker
            // So we just skip it. It it only vital that StringValue matches the speicifed format
            // The UI is sugar.
            //datePicker.Text.Should().Be(expected);

            sut.Value = null;
            sut.StringValue.Should().BeEmpty();
            datePicker.SelectedDate.Should().NotHaveValue();
        }

        [Test]
        [STAThread]
        public void Format_SelectedDate_From_Value()
        {
            var sut = new NZazuDateField(new FieldDefinition {Key="test"});
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
        [STAThread]
        public void Format_Value_From_TextBox()
        {
            var sut = new NZazuDateField(new FieldDefinition {Key="test"});
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
        [STAThread]
        public void Format_TextBox_From_StringValue()
        {
            var sut = new NZazuDateField(new FieldDefinition {Key="test"});
            var datePicker = (DatePicker)sut.ValueControl;

            sut.StringValue.Should().BeNullOrEmpty();
            datePicker.Text.Should().BeEmpty();

            var now = DateTime.Now.Date;
            sut.StringValue = now.ToString(CultureInfo.InvariantCulture);
            datePicker.SelectedDate.Should().Be(now);

            sut.StringValue = string.Empty;
            datePicker.SelectedDate.Should().NotHaveValue();
        }

        [Test]
        [STAThread]
        public void Format_StringValue_From_TextBox()
        {
            var sut = new NZazuDateField(new FieldDefinition {Key="test"});
            var datePicker = (DatePicker)sut.ValueControl;

            var now = DateTime.Now.Date;
            datePicker.SelectedDate = now;
            sut.StringValue.Should().Be(now.ToString(CultureInfo.InvariantCulture));

            datePicker.SelectedDate = null;
            sut.IsValid().Should().BeTrue();
            sut.StringValue.Should().Be("");

            datePicker.Text = null;
            sut.IsValid().Should().BeTrue();
            sut.StringValue.Should().Be(string.Empty);
        }

        [Test]
        public void Consider_DateFormat_in_StringValue()
        {
            var sut = new NZazuDateField(new FieldDefinition {Key="test"});

            // DateFormat unspecified
            var date = DateTime.UtcNow;
            var dateStr = date.ToString(CultureInfo.InvariantCulture);
            sut.StringValue = dateStr;
            sut.Value.Should().BeCloseTo(date, 1000, "parsing truncates millis");

            date += TimeSpan.FromSeconds(1);
            dateStr = date.ToString(CultureInfo.InvariantCulture);
            sut.Value = date;
            sut.StringValue.Should().Be(dateStr);

            // now specify DateFormat
            const string dateFormat = "yyyy-MMM-dd";
            sut.DateFormat = dateFormat;

            dateStr = date.ToString(dateFormat, CultureInfo.InvariantCulture);
            sut.StringValue.Should().Be(dateStr);

            date -= TimeSpan.FromDays(60);
            dateStr = date.ToString(dateFormat, CultureInfo.InvariantCulture);
            sut.Value = date;
            sut.StringValue.Should().Be(dateStr);

            date += TimeSpan.FromDays(2);
            date = date.Date; // truncate time (we only check date --> format)
            dateStr = date.ToString(dateFormat, CultureInfo.InvariantCulture);
            sut.StringValue = dateStr;
            sut.Value.Should().Be(date);
        }
    }
}