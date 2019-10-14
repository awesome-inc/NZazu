using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuBoolField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuBoolField_Should
    {
        [ExcludeFromCodeCoverage]
        private object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        [STAThread]
        public void Be_Creatable()
        {
            var sut = new NZazuBoolField(new FieldDefinition {Key = "bool01"}, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        [STAThread]
        public void Not_Create_Empty_Label()
        {
            var sut = new NZazuBoolField(new FieldDefinition {Key = "bool01"}, ServiceLocator);
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        [STAThread]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuBoolField(new FieldDefinition {Key = "bool01", Prompt = "superhero"}, ServiceLocator);

            var label = (Label) sut.LabelControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Definition.Prompt);
        }

        [Test]
        [STAThread]
        public void Create_ValueControl_Matching_Description()
        {
            var sut = new NZazuBoolField(new FieldDefinition
            {
                Key = "bool01",
                Hint = "superhero",
                Description = "check this if you are a registered superhero"
            }, ServiceLocator);

            var label = (CheckBox) sut.ValueControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Definition.Hint);
            label.ToolTip.Should().Be(sut.Definition.Description);
        }

        [Test]
        [STAThread]
        public void Create_ValueControl_Even_If_Empty_Hint()
        {
            var sut = new NZazuBoolField(new FieldDefinition {Key = "bool01"}, ServiceLocator);

            var label = (CheckBox) sut.ValueControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Definition.Hint);
        }

        [Test]
        [STAThread]
        public void Get_Set_Value_should_propagate_to_ValueControl()
        {
            var sut = new NZazuBoolField(new FieldDefinition {Key = "bool01"}, ServiceLocator);
            sut.GetValue().Should().BeEmpty();
            var checkBox = (CheckBox) sut.ValueControl;

            // set
            sut.Value = true;
            checkBox.IsChecked.Should().Be(true);
            sut.GetValue().Should().Be("True");

            sut.SetValue("false");
            checkBox.IsChecked.Should().Be(false);
            sut.GetValue().Should().Be("False");

            sut.SetValue("foobar");
            checkBox.IsChecked.Should().NotHaveValue();

            // get
            checkBox.IsChecked = true;
            sut.GetValue().Should().Be("True");

            checkBox.IsChecked = false;
            sut.GetValue().Should().Be("False");

            checkBox.IsChecked = null;
            sut.GetValue().Should().BeEmpty();
        }

        [Test]
        [STAThread]
        public void Support_ThreeState_by_default()
        {
            var sut = new NZazuBoolField(new FieldDefinition {Key = "bool01"}, ServiceLocator);

            ((CheckBox) sut.ValueControl).IsThreeState.Should().BeTrue();
        }

        [Test]
        [STAThread]
        public void Center_checkboxes_vertically()
        {
            var sut = new NZazuBoolField(new FieldDefinition {Key = "bool01"}, ServiceLocator);
            var checkBox = (CheckBox) sut.ValueControl;
            checkBox.VerticalContentAlignment.Should().Be(VerticalAlignment.Center);
        }
    }
}