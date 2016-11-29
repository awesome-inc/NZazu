using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;

#pragma warning disable 618
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
        [STAThread]
        public void Serialize_And_Deserialize()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition {Key = "table01_field01", Type = "string"}
                }
            });
            sut.FieldFactory = new NZazuFieldFactory();
            var justToMakeTheCall = sut.ValueControl;

            const string value = "<items />";
            sut.StringValue = value;
            sut.StringValue.Should().Be(value);

            sut.Validate().IsValid.Should().BeTrue();
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
        public void Be_Editable_To_Make_Sure_GetSet_StringValue_Are_Called()
        {
            var sut = new NZazuDataTableField(new FieldDefinition { Key = "table01" });
            sut.IsEditable.Should().BeTrue();
        }

        [Test]
        [STAThread]
        public void Hanlde_Add()
        {
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "key"
            });

        }

    }
}