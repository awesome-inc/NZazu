using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof (NZazuLabelField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuLabelField_Should
    {
        [Test]
        [STAThread]
        public void Create_ValueControl_Matching_Description()
        {
            var sut = new NZazuLabelField("test", new FieldDefinition())
            {
                Description = "superhero is alive"
            };

            var label = (Label)sut.ValueControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Description);
        }

        [Test]
        public void Not_Create_ValueControl_On_Empty_Description()
        {
            var sut = new NZazuLabelField("test", new FieldDefinition());
            sut.Description.Should().BeNullOrWhiteSpace();
            var label = (Label)sut.ValueControl;
            label.Should().BeNull();
        }

        [Test]
        public void Return_null_StringValue_and_not_set_StringValue()
        {
            var sut = new NZazuLabelField("test", new FieldDefinition());
            sut.StringValue.Should().BeNull();
            sut.StringValue = "foobar";
            sut.StringValue.Should().BeNull();
        }

        [Test]
        public void Not_be_Editable()
        {
            var sut = new NZazuLabelField("test", new FieldDefinition());
            sut.IsEditable.Should().BeFalse();
        }
    }
}