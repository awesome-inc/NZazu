using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Fields.Controls;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

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
            var factory = new NZazuFieldFactory();
            var data = new Dictionary<string, string> { { "table01_field01__1", "hello" }, { "table01_field01__2", "world" } };
            var dataSerialized = factory.Serializer.Serialize(data);

            // ReSharper disable once UseObjectOrCollectionInitializer
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition {Key = "table01_field01", Type = "string"}
                }
            });
            sut.FieldFactory = factory;
            // ReSharper disable once UnusedVariable
            var justToMakeTheCall = sut.ValueControl;

            sut.StringValue = dataSerialized;
            var actual = factory.Serializer.Deserialize(sut.StringValue);
            foreach (var dta in data)
                actual.Should().Contain(dta);

            sut.Validate().IsValid.Should().BeTrue();
            ((DynamicDataTable)sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(3);
        }

        [Test]
        [STAThread]
        public void Validate()
        {
            var factory = new NZazuFieldFactory();
            var data = new Dictionary<string, string> { { "table01_field01__1", "" }, { "table01_field01__2", "world" } };
            var dataSerialized = factory.Serializer.Serialize(data);

            // ReSharper disable once UseObjectOrCollectionInitializer
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = "table01_field01", Type = "string",
                        Checks = new []{ new CheckDefinition {Type = "required" } }
                    }
                }
            });
            sut.FieldFactory = factory;
            // ReSharper disable once UnusedVariable
            var justToMakeTheCall = sut.ValueControl;

            sut.StringValue = dataSerialized;
            sut.Validate().IsValid.Should().BeFalse();

            // now change the data
            data = new Dictionary<string, string> { { "table01_field01__1", "hello" }, { "table01_field01__2", "world" } };
            dataSerialized = factory.Serializer.Serialize(data);
            sut.StringValue = dataSerialized;
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
        public void Handle_Add_An_Tab()
        {
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "key",
                Type = "datatable",
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = "cell01",
                        Type = "string"
                    }
                }
            })
            {
                FieldFactory = new NZazuFieldFactory()
            };

            var ctrl = (DynamicDataTable)sut.ValueControl;
            var lastadded = ctrl.LayoutGrid.Children[1];
            lastadded.Should().NotBeNull();

            // lets see if it adds a row
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(2);
            sut.LastAddedFieldOnPreviewKeyDown(lastadded, new KeyEventArgs(
                    Keyboard.PrimaryDevice,
                    new FakePresentationSource(), 0, Key.Tab));
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(3);
        }

        [Test]
        [STAThread]
        public void Handle_Delete_Button()
        {
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "key",
                Type = "datatable",
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = "cell01",
                        Type = "string"
                    }
                }
            })
            {
                FieldFactory = new NZazuFieldFactory()
            };
            var ctrl = (DynamicDataTable)sut.ValueControl;

            var data = new Dictionary<string, string> { { "table01_field01__1", "hello" }, { "table01_field01__2", "world" } };
            var dataSerialized = sut.FieldFactory.Serializer.Serialize(data);
            sut.StringValue = dataSerialized;

            var lastadded = ctrl.LayoutGrid.Children[1];
            lastadded.Should().NotBeNull();

            // lets see if it adds a row
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(3);
            sut.DeleteRow(lastadded);
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [STAThread]
        public void Handle_Add_Button()
        {
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "key",
                Type = "datatable",
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = "cell01",
                        Type = "string"
                    }
                }
            })
            {
                FieldFactory = new NZazuFieldFactory()
            };
            var ctrl = (DynamicDataTable)sut.ValueControl;

            var data = new Dictionary<string, string> { { "table01_field01__1", "hello" }, { "table01_field01__2", "world" } };
            var dataSerialized = sut.FieldFactory.Serializer.Serialize(data);
            sut.StringValue = dataSerialized;

            var lastadded = ctrl.LayoutGrid.Children[2];
            lastadded.Should().NotBeNull();

            // lets see if it adds a row
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(3);
            sut.AddRowAbove(lastadded);
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(4);
        }

        [ExcludeFromCodeCoverage]
        [Because("just for event testing")]
        private class FakePresentationSource : PresentationSource
        {
            protected override CompositionTarget GetCompositionTargetCore()
            {
                return null;
            }

            public override Visual RootVisual { get; set; }

            public override bool IsDisposed => false;
        }
    }
}