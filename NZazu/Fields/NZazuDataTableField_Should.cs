using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuDataTableField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuDataTableField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuDataTableField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Should().BeAssignableTo<INZazuWpfControlContainer>();
            sut.Type.Should().Be("datatable");
        }

        [Test]
        public void Not_support_direct_binding_or_validation()
        {
            var sut = new NZazuDataTableField("key");
            sut.ContentProperty.Should().Be(null);
        }

        [Test]
        [STAThread]
        public void Create_ContentControl()
        {
            var sut = new NZazuDataTableField("key");
            var contentControl = (ContentControl)sut.ValueControl;

            contentControl.Should().NotBeNull();

            contentControl.Focusable.Should().BeFalse("group fields should not have a tab stop of their own");
        }

        [Test]
        public void Not_be_Editable()
        {
            var sut = new NZazuDataTableField("test");
            sut.IsEditable.Should().BeFalse();
        }
    }
}