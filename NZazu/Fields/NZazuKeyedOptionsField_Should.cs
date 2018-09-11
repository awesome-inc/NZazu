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
            var sut = new NZazuKeyedOptionsField(new FieldDefinition { Key = "test" }, type => null);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test(Description = "https://github.com/awesome-inc/NZazu/issues/68")]
        [STAThread]
        public void Create_ComboBox()
        {
            var definition = new FieldDefinition { Key = "test", Description = "description" };
            var sut = new NZazuKeyedOptionsField(definition, type => null);

            sut.ContentProperty.Should().Be(ComboBox.TextProperty);
            var control = (ComboBox)sut.ValueControl;
            control.Should().NotBeNull();

            control.ToolTip.Should().Be(sut.Definition.Description);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Identify_Value_with_StringValue()
        {
            var sut = new NZazuKeyedOptionsField(new FieldDefinition { Key = "test", Description = "description" }, type => null);

            sut.Value.Should().BeNull();
            sut.GetStringValue().Should().Be(sut.Value);

            sut.SetStringValue( "1");
            sut.Value.Should().Be(sut.GetStringValue());

            sut.Value = "2";
            sut.GetStringValue().Should().Be(sut.Value);
        }
    }
}