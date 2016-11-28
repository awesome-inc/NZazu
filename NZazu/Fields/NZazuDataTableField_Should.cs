using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;

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
            var sut = new NZazuDataTableField(new FieldDefinition { Key = "table01" });

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Type.Should().Be("datatable");
        }

        [Test]
        public void Not_Support_direct_binding_or_validation_Right_Now()
        {
            var sut = new NZazuDataTableField(new FieldDefinition { Key = "table01" });
            sut.ContentProperty.Should().Be(null);
        }

        [Test]
        [STAThread]
        public void Create_ContentControl()
        {
            var sut = new NZazuDataTableField(new FieldDefinition { Key = "table01" });
            var contentControl = (ContentControl)sut.ValueControl;

            contentControl.Should().NotBeNull();

            contentControl.Focusable.Should().BeFalse("group fields should not have a tab stop of their own");
        }

        [Test]
        public void Not_be_Editable()
        {
            var sut = new NZazuDataTableField(new FieldDefinition { Key = "table01" });
            sut.IsEditable.Should().BeFalse();
        }

        [Test]
        [STAThread]
        public void Hanlde_Add()
        {
            var behaviorFactory = Substitute.For<INZazuWpfFieldBehaviorFactory>();
            var checkFactory = Substitute.For<ICheckFactory>();
            var serializer = Substitute.For<INZazuDataSerializer>();

            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "key",
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = "test01",
                        Type = "string"
                    }
                }
            })
            { FieldFactory = new NZazuFieldFactory(behaviorFactory, checkFactory, serializer) };
            var ctrl = sut.Value;

            sut.AddBtnOnClick(null, null);
        }
    }
}