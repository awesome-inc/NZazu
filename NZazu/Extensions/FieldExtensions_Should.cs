using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts.Checks;

namespace NZazu.Extensions
{
    [TestFixtureFor(typeof(FieldExtensions))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class FieldExtensions_Should
    {
        [Test]
        public void Return_False_If_Validate_Has_Exception()
        {
            var field = Substitute.For<INZazuWpfField>();
            field.Validate().Returns(new ValueCheckResult(false, new Exception("I am invalid")));

            field.IsValid().Should().BeFalse();

            field.ReceivedWithAnyArgs().Validate();
        }

        [Test]
        public void Return_True_If_Validate()
        {
            var field = Substitute.For<INZazuWpfField>();
            field.Validate().Returns(ValueCheckResult.Success);

            field.IsValid().Should().BeTrue();

            field.ReceivedWithAnyArgs().Validate();
        }

        [Test]
        [STAThread]
        public void Return_ReadOnly_if_not_editable_not_enabled_or_read_only()
        {
            var field = Substitute.For<INZazuWpfField>();
            field.IsEditable.Returns(true);
            field.IsReadOnly().Should().BeTrue("field has no value control");

            var label = new Label();
            field.ValueControl.Returns(label);

            label.IsEnabled.Should().BeTrue();
            field.IsReadOnly().Should().Be(false, "field.control is enabled");

            field.IsEditable.Returns(false);
            field.IsReadOnly().Should().BeTrue("field is not editable");
            field.IsEditable.Returns(true);

            label.IsEnabled = false;
            field.IsReadOnly().Should().Be(true, "field.control is disabled");


            var textBox = new TextBox();
            field.ValueControl.Returns(textBox);
            textBox.IsEnabled.Should().BeTrue();
            textBox.IsReadOnly.Should().BeFalse();
            field.IsReadOnly().Should().Be(false, "field.control is read only");

            textBox.IsReadOnly = true;
            field.IsReadOnly().Should().Be(true, "field.control is read only");
        }

        [Test]
        [STAThread]
        public void Set_ReadOnly_if_editable_via_enabled_or_read_only()
        {
            var field = Substitute.For<INZazuWpfField>();
            field.IsEditable.Returns(true);
            var label = new Label();
            field.ValueControl.Returns(label);

            field.IsReadOnly().Should().BeFalse();
            field.SetReadOnly(true);
            field.IsReadOnly().Should().BeTrue();
            label.IsEnabled.Should().BeFalse();

            field.SetReadOnly(false);
            field.IsReadOnly().Should().BeFalse();
            label.IsEnabled.Should().BeTrue();

            var textBox = new TextBox();
            field.ValueControl.Returns(textBox);
            field.IsReadOnly().Should().BeFalse();
            field.SetReadOnly(true);
            field.IsReadOnly().Should().BeTrue();
            textBox.IsReadOnly.Should().BeTrue();

            field.SetReadOnly(false);
            field.IsReadOnly().Should().BeFalse();
            textBox.IsReadOnly.Should().BeFalse();
        }

        [Test(Description = "especially for group fields")]
        [STAThread]
        public void Not_Set_ReadOnly_if_not_editable()
        {
            var field = Substitute.For<INZazuWpfFieldContainer>();
            field.IsEditable.Returns(true);

            var control = new ContentControl();
            field.ValueControl.Returns(control);

            field.IsReadOnly().Should().BeTrue();
            field.SetReadOnly(false);
            field.IsReadOnly().Should().BeTrue();
        }
    }
}