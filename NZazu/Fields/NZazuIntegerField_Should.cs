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
    [TestFixtureFor(typeof(NZazuIntegerField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuIntegerField_Should
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
            var sut = new NZazuIntegerField(new FieldDefinition {Key = "test"}, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        [STAThread]
        public void Create_Control_With_ToolTip_Matching_Description()
        {
            var sut = new NZazuIntegerField(new FieldDefinition
            {
                Key = "test",
                Hint = "superhero",
                Description = "check this if you are a registered superhero"
            }, ServiceLocator);

            var textBox = (TextBox) sut.ValueControl;
            textBox.Should().NotBeNull();
            textBox.Text.Should().BeEmpty();
            textBox.ToolTip.Should().Be(sut.Definition.Description);
        }

        [Test]
        [STAThread]
        public void Format_TextBox_From_Value()
        {
            var sut = new NZazuIntegerField(new FieldDefinition {Key = "test"}, ServiceLocator);
            var textBox = (TextBox) sut.ValueControl;

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
            var sut = new NZazuIntegerField(new FieldDefinition {Key = "test"}, ServiceLocator);
            var textBox = (TextBox) sut.ValueControl;

            sut.Value.Should().NotHaveValue();
            textBox.Text.Should().BeEmpty();

            textBox.Text = "7";
            sut.Value.Should().Be(7);

            textBox.Text = "-12";
            sut.Value.Should().Be(-12);

            textBox.Text = "foo bar";
            sut.IsValid().Should().BeFalse();
            sut.Value.Should().Be(-12, "WPF binding cannot sync value");

            textBox.Text = "";
            sut.IsValid().Should().BeTrue();
            sut.Value.Should().NotHaveValue();
        }

        [Test]
        [STAThread]
        public void Format_TextBox_From_StringValue()
        {
            var sut = new NZazuIntegerField(new FieldDefinition {Key = "test"}, ServiceLocator);
            var textBox = (TextBox) sut.ValueControl;

            sut.GetValue().Should().BeNullOrEmpty();
            textBox.Text.Should().BeEmpty();

            sut.SetValue("42");
            textBox.Text.Should().Be("42");

            sut.SetValue("-23");
            textBox.Text.Should().Be("-23");

            sut.SetValue(null);
            textBox.Text.Should().Be(string.Empty);
        }

        [Test]
        [STAThread]
        public void Format_StringValue_From_TextBox()
        {
            var sut = new NZazuIntegerField(new FieldDefinition {Key = "test"}, ServiceLocator);
            var textBox = (TextBox) sut.ValueControl;

            textBox.Text = "7";
            sut.GetValue().Should().Be("7");

            textBox.Text = "-12";
            sut.GetValue().Should().Be("-12");

            textBox.Text = "foo bar";
            sut.IsValid().Should().BeFalse();
            sut.GetValue().Should().Be("-12", "WPF binding cannot sync value");

            textBox.Text = "";
            sut.IsValid().Should().BeTrue();
            sut.GetValue().Should().Be("");

            // ReSharper disable once AssignNullToNotNullAttribute
            textBox.Text = null;
            sut.IsValid().Should().BeTrue();
            sut.GetValue().Should().Be(string.Empty);
        }
    }
}