using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuDoubleField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuDoubleField_Should
    {
        [ExcludeFromCodeCoverage]
        private object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuDoubleField(new FieldDefinition {Key = "key"}, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        [STAThread]
        public void Create_Control_With_ToolTip_Matching_Description()
        {
            var sut = new NZazuDoubleField(new FieldDefinition
            {
                Key = "key",
                Hint = "superhero",
                Description = "check this if you are a registered superhero"
            }, ServiceLocator);

            var textBox = (TextBox) sut.ValueControl;
            textBox.Should().NotBeNull();
            textBox.Text.Should().BeEmpty();
            textBox.ToolTip.Should().Be(sut.Definition.Description);
        }

        [Test]
        [SetUICulture("en-US")]
        [STAThread]
        public void Support_Format()
        {
            var sut = new NZazuDoubleField(new FieldDefinition {Key = "key"}, ServiceLocator);
            sut.Definition.Settings["Format"] = "0.##";
            var control = (TextBox) sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            control.Text.Should().BeEmpty();

            sut.Value = 1.0 / 3.0;
            control.Text.Should().Be("0.33");

            sut.Value = 1.4;
            control.Text.Should().Be("1.4");

            sut.Value = null;
            control.Text.Should().BeEmpty();
        }

        [Test]
        [SetUICulture("en-US")]
        [STAThread]
        public void Format_TextBox_From_Value()
        {
            var sut = new NZazuDoubleField(new FieldDefinition {Key = "key"}, ServiceLocator);
            var control = (TextBox) sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            control.Text.Should().BeEmpty();

            sut.Value = 1.0 / 3.0;
            control.Text.Should().Be("0.333333333333333");

            sut.Value = 1.4;
            control.Text.Should().Be("1.4");

            sut.Value = -2.34;
            control.Text.Should().Be("-2.34");

            sut.Value = null;
            control.Text.Should().BeEmpty();
        }

        [Test]
        [SetUICulture("en-US")]
        [STAThread]
        public void Format_Value_From_TextBox()
        {
            var sut = new NZazuDoubleField(new FieldDefinition {Key = "key"}, ServiceLocator);
            var control = (TextBox) sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            control.Text.Should().BeEmpty();

            control.Text = "1.";
            sut.Value.Should().BeApproximately(1.0, double.Epsilon);
            control.Text.Should().Be("1.");

            // ReSharper disable once AssignNullToNotNullAttribute
            control.Text = null;
            sut.IsValid().Should().BeTrue();
            sut.Value.Should().NotHaveValue();
        }

        [Test]
        [SetUICulture("en-US")]
        [STAThread]
        public void Format_TextBox_From_StringValue()
        {
            var sut = new NZazuDoubleField(new FieldDefinition {Key = "key"}, ServiceLocator);
            var control = (TextBox) sut.ValueControl;

            sut.GetValue().Should().BeNullOrEmpty();
            control.Text.Should().BeEmpty();

            sut.SetValue("1.4");
            control.Text.Should().Be("1.4");

            sut.SetValue("");
            control.Text.Should().BeEmpty();
        }

        [Test]
        [SetUICulture("en-US")]
        [STAThread]
        public void Format_StringValue_From_TextBox()
        {
            var sut = new NZazuDoubleField(new FieldDefinition {Key = "key"}, ServiceLocator);
            var control = (TextBox) sut.ValueControl;

            control.Text = "1.4";
            sut.GetValue().Should().Be("1.4");

            control.Text = string.Empty;
            sut.IsValid().Should().BeTrue();
            sut.GetValue().Should().Be("");

            // ReSharper disable once AssignNullToNotNullAttribute
            control.Text = null;
            sut.IsValid().Should().BeTrue();
            sut.GetValue().Should().Be(string.Empty);
        }

        #region converter tests

        [Test]
        [SetUICulture("de-DE")]
        public void Have_A_Fancy_Converter_DE()
        {
            Have_A_Fency_Converter();
        }

        [Test]
        [SetUICulture("en-US")]
        public void Have_A_Fancy_Converter_US()
        {
            Have_A_Fency_Converter();
        }

        [Test]
        [SetUICulture("en-GB")]
        public void Have_A_Fancy_Converter_GB()
        {
            Have_A_Fency_Converter();
        }

        [Test]
        [SetUICulture("fr-FR")]
        public void Have_A_Fancy_Converter_FR()
        {
            Have_A_Fency_Converter();
        }

        private static void Have_A_Fency_Converter()
        {
            var culture = Thread.CurrentThread.CurrentUICulture;
            var separator = culture.NumberFormat.CurrencyDecimalSeparator;
            var converter = new NZazuDoubleField.DoubleToStringConverter(culture: culture);

            // lets do some fake edits which means the string is entered char by char
            VerifyConvert(converter, "", null);
            VerifyConvert(converter, "3", 3);
            VerifyConvert(converter, "35", 35);
            VerifyConvert(converter, "35" + separator, 35);
            VerifyConvert(converter, "35" + separator + "3", 35.3);
            VerifyConvert(converter, "35" + separator, 35);
            VerifyConvert(converter, "35" + separator + separator, null);
            VerifyConvert(converter, "35" + separator, 35);
            VerifyConvert(converter, "35" + separator + "3", 35.3);
        }

        [Test]
        [SetUICulture("en-GB")]
        public void Have_A_Fency_Converter_Which_Prefers_Injected_Culture()
        {
            var cultureDe = new CultureInfo("de-DE");
            var converter = new NZazuDoubleField.DoubleToStringConverter(culture: cultureDe);

            converter.Convert(23.34, null, null, new CultureInfo("en-US")).Should().Be("23,34");
        }

        [Test]
        [SetUICulture("en-GB")]
        public void Have_A_Fency_Converter_Which_Prefers_Parameter_Culture()
        {
            var cultureDe = new CultureInfo("de-DE");
            var converter = new NZazuDoubleField.DoubleToStringConverter();

            converter.Convert(23.34, null, null, cultureDe).Should().Be("23,34");
        }

        private static void VerifyConvert(IValueConverter converter, string input, double? expected)
        {
            var value = (double?) converter.ConvertBack(input, null, null, null);
            value.Should().Be(expected);
            var text = (string) converter.Convert(value, null, null, null);
            text.Should().Be(input);
        }

        #endregion
    }
}