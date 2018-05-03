using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuKeyedOptionsField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuKeyedOptionsField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuKeyedOptionsField(new FieldDefinition { Key = "test" });

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Type.Should().Be("keyedoption");
        }

        [Test(Description = "https://github.com/awesome-inc/NZazu/issues/68")]
        [STAThread]
        public void Create_ComboBox()
        {
            var definition = new FieldDefinition { Key = "test", Description = "description" };
            var sut = new NZazuKeyedOptionsField(definition);

            sut.ContentProperty.Should().Be(ComboBox.TextProperty);
            var control = (ComboBox)sut.ValueControl;
            control.Should().NotBeNull();

            control.ToolTip.Should().Be(sut.Description);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Identify_Value_with_StringValue()
        {
            var sut = new NZazuKeyedOptionsField(new FieldDefinition { Key = "test", Description = "description" });

            sut.Value.Should().BeNull();
            sut.StringValue.Should().Be(sut.Value);

            sut.StringValue = "1";
            sut.Value.Should().Be(sut.StringValue);

            sut.Value = "2";
            sut.StringValue.Should().Be(sut.Value);
        }
    }
}