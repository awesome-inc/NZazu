using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixtureFor(typeof (XceedFieldFactory))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedFieldFactory_Should
    {
        [Test]
        [STAThread]
        public void Create_WatermarkTextbox()
        {
            var sut = new XceedFieldFactory();
            var fieldDefinition = new FieldDefinition
            {
                Key = "login",
                Type = "string",
                Prompt = "login",
                Hint = "Enter user name",
                Description = "Your User name"
            };
            var field = sut.CreateField(fieldDefinition);
            field.Should().BeOfType<XceedTextBoxField>();

            var textBox = (WatermarkTextBox) field.ValueControl;
            textBox.Watermark.Should().Be(fieldDefinition.Hint);
        }

        [Test]
        [STAThread]
        public void Create_DateTimePicker()
        {
            var sut = new XceedFieldFactory();
            var fieldDefinition = new FieldDefinition
            {
                Key = "birthday",
                Type = "date",
                Prompt = "Date of Birth",
                Hint = "Enter date of birth",
                Description = "Your birthday",
            };
            var field = sut.CreateField(fieldDefinition);
            field.Should().BeOfType<XceedDateTimeField>();

            var datePicker = (DateTimePickerWithUpdate) field.ValueControl;
            datePicker.Watermark.Should().Be(fieldDefinition.Hint);
            datePicker.FormatString.Should().BeNull();

            const string dateFormat = "yyyy_MM_dd";
            fieldDefinition.Settings = new Dictionary<string, string> { { "Format", dateFormat } };

            field = sut.CreateField(fieldDefinition);
            datePicker = (DateTimePickerWithUpdate)field.ValueControl;
            datePicker.FormatString.Should().Be(dateFormat);
        }

        [Test]
        public void Override_Double()
        {
            var sut = new XceedFieldFactory();
            var fieldDefinition = new FieldDefinition
            {
                Key = "weight",
                Type = "double",
                Prompt = "Weight",
                Hint = "Enter weight",
                Description = "Your weight",
            };
            var field = sut.CreateField(fieldDefinition);
            field.Should().BeOfType<XceedDoubleField>();
        }

        [Test]
        public void Override_Integer()
        {
            var sut = new XceedFieldFactory();
            var fieldDefinition = new FieldDefinition
            {
                Key = "age",
                Type = "int",
                Prompt = "Age",
                Hint = "Enter Age",
                Description = "Your Age",
            };
            var field = sut.CreateField(fieldDefinition);
            field.Should().BeOfType<XceedIntegerField>();
        }

        [Test]
        [STAThread]
        public void Support_RichTextBox()
        {
            var sut = new XceedFieldFactory();
            var fieldDefinition = new FieldDefinition
            {
                Key = "notes",
                Type = "richtext",
                Prompt = "Notes",
                Hint = "Enter Notes",
                Description = "Notes",
            };
            var field = (XceedRichTextField)sut.CreateField(fieldDefinition);
            field.Should().NotBeNull();


            var textBox = (RichTextBox)field.ValueControl;
            textBox.Should().NotBeNull();
            textBox.ToolTip.Should().Be(field.Definition.Description);
        }
    }
}