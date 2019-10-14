using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
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
    [TestFixtureFor(typeof(NZazuOptionsField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuOptionsField_Should
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
            var sut = new NZazuOptionsField(new FieldDefinition {Key = "key"}, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test(Description = "https://github.com/awesome-inc/NZazu/issues/68")]
        [STAThread]
        public void Create_ComboBox()
        {
            var sut = new NZazuOptionsField(new FieldDefinition {Key = "key", Description = "description"},
                ServiceLocator);

            sut.ContentProperty.Should().Be(ComboBox.TextProperty);
            var control = (ComboBox) sut.ValueControl;
            control.Should().NotBeNull();

            control.ToolTip.Should().Be(sut.Definition.Description);
        }

        [Test(Description = "https://github.com/awesome-inc/NZazu/issues/68")]
        [STAThread]
        public void Reflect_changing_Value_in_TextProperty()
        {
            var sut = new NZazuOptionsField(new FieldDefinition {Key = "key"}, ServiceLocator)
            {
                Options = new[] {"1", "2", "3", "4", "5"}
            };

            var control = (ComboBox) sut.ValueControl;
            control.Items.Should().BeEquivalentTo(sut.Options);
            control.IsEditable.Should().BeFalse();

            sut.Value.Should().BeNull();
            control.SelectedItem.Should().BeNull();

            // value -> selected item
            var expected = sut.Options.First();
            sut.Value = expected;
            control.Text.Should().Be(expected);

            // selected item -> value
            expected = sut.Options.Last();
            control.SelectedItem = expected;
            sut.Value.Should().Be(expected);

            // custom values
            expected = "42";
            sut.Value = expected;
            sut.Value.Should().Be(expected);
            control.Text.Should().Be(expected);

            control.SelectedItem = expected;
            control.Text.Should().Be(expected);
        }

        [Test]
        public void Identify_Value_with_StringValue()
        {
            var sut = new NZazuOptionsField(new FieldDefinition {Key = "key"}, ServiceLocator);

            sut.Value.Should().BeNull();
            sut.GetValue().Should().Be(sut.Value);

            sut.SetValue("1");
            sut.Value.Should().Be(sut.GetValue());

            sut.Value = "2";
            sut.GetValue().Should().Be(sut.Value);
        }
    }
}