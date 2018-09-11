using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuGroupField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuGroupField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuGroupField(new FieldDefinition { Key = "key" }, type => null);
            sut.Should().NotBeNull();

            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Should().BeAssignableTo<INZazuWpfFieldContainer>();

            // at least nobodyd cares about these values
            sut.GetStringValue().Should().BeNullOrEmpty();
            sut.SetStringValue("foo");
            sut.GetStringValue().Should().Be("foo");
        }

        [Test]
        public void Not_support_direct_binding_or_validation()
        {
            var sut = new NZazuGroupField(new FieldDefinition { Key = "key" }, type => null);
            sut.ContentProperty.Should().Be(null);
        }

        [Test]
        [STAThread]
        public void Create_ContentControl()
        {
            var sut = new NZazuGroupField(new FieldDefinition { Key = "key" }, type => null);
            var contentControl = (ContentControl)sut.ValueControl;

            contentControl.Should().NotBeNull();

            contentControl.Focusable.Should().BeFalse("group fields should not have a tab stop of their own");
        }

        [Test]
        [STAThread]
        public void Create_Groupbox()
        {
            var sut = new NZazuGroupField(new FieldDefinition { Key = "key", Description = "Header" }, type => null);
            var contentControl = (GroupBox)sut.ValueControl;

            contentControl.Should().NotBeNull();

            contentControl.Focusable.Should().BeFalse("group fields should not have a tab stop of their own");
        }

        [Test]
        public void Not_be_Editable()
        {
            var sut = new NZazuGroupField(new FieldDefinition { Key = "test" }, type => null);
            sut.IsEditable.Should().BeFalse();
        }
    }
}