using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Fields.Controls;

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


        private static dynamic GetField()
        {
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition {Key = "table01_field01", Type = "string"},
                    new FieldDefinition {Key = "table01_field02", Type = "bool"}
                }
            });

            // create factory later but before "ValueControl" access
            var _factory = new NZazuFieldFactory();
            sut.FieldFactory = _factory;
            // don't create the control by invoking the getter.
            //var control = sut.ValueControl;
            //control.Should().NotBeNull();

            // now lets create some testdata and return it for multiple tests
            var data = new Dictionary<string, string>
            {
                { "table01_field01__1", "Hello" },
                { "table01_field02__1", "True" },
                { "table01_field01__2", "World" },
                { "table01_field02__2", "False" },
                { "table01_field01__3", "42" },
                { "table01_field02__3", "" }
            };

            return new
            {
                Field = sut,
                Data = data,
                Factory = _factory
            };
        }

        [Test]
        [STAThread]
        public void Serialize_And_Deserialize()
        {
            var testData = GetField();
            var sut = (NZazuDataTableField)testData.Field;
            var factory = (INZazuWpfFieldFactory)testData.Factory;
            var data = (Dictionary<string, string>)testData.Data;

            // lets assign the data and do some tests
            sut.StringValue = factory.Serializer.Serialize(data);
            var actual = factory.Serializer.Deserialize(sut.StringValue);
            foreach (var dta in data)
                actual.Should().Contain(dta);

            sut.Validate().IsValid.Should().BeTrue();
            ((DynamicDataTable)sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(4);
        }

        [Test]
        [STAThread]
        public void Serialize_And_Deserialize_Multiple()
        {
            var testData = GetField();
            var sut = (NZazuDataTableField)testData.Field;
            var factory = (INZazuWpfFieldFactory)testData.Factory;
            var data = (Dictionary<string, string>)testData.Data;
            var sd = factory.Serializer.Serialize(data);

            // lets assign the data
            sut.StringValue = sd;

            // lets assign the data again
            var testData2 = GetField();
            var sut2 = (NZazuDataTableField)testData2.Field;
            sut2.StringValue = sd;
        }

        [Test]
        [STAThread]
        public void Serialize_And_Deserialize_Null_Rows_To_Null()
        {
            var factory = new NZazuFieldFactory();
            var data = new Dictionary<string, string> { { "table01_field01__1", null }, { "table01_field01__2", null } };
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
            sut.StringValue = dataSerialized;

            // test the returned data
            var actual = factory.Serializer.Deserialize(sut.StringValue);
            actual.Count.Should().Be(0);

            sut.Validate().IsValid.Should().BeTrue();
            ((DynamicDataTable)sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("<items/>")]
        [STAThread]
        public void Deserialize_Empty_Data(string data)
        {
            var factory = new NZazuFieldFactory();

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

            sut.StringValue = data;
            var actual = factory.Serializer.Deserialize(sut.StringValue);

            actual.Should().NotBeNull();

            ((DynamicDataTable)sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [TestCase("this should be ignored")]
        [STAThread]
        public void Ignore_Exception_On_Deserialize_Invalid_Data(string data)
        {
            var factory = new NZazuFieldFactory();

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

            Action act = () => { sut.StringValue = data; };

            act.ShouldNotThrow<Exception>();

            ((DynamicDataTable)sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
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
    }
}