using System;
using System.Collections.Generic;
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
    [Ignore("skip for now because this takes long to tes for some reasons")]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
        public void Assign_Data_Multiple_Times()
        {
            var testData = GetField();
            var sut = (NZazuDataTableField)testData.Field;
            var factory = (INZazuWpfFieldFactory)testData.Factory;
            var data = (Dictionary<string, string>)testData.Data;
            var sd = factory.Serializer.Serialize(data);

            // lets assign the data
            sut.StringValue = sd;
            ((DynamicDataTable)sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(4);

            // lets assign other data
            data = new Dictionary<string, string>
            {
                { "table01_field01__1", "Hello" },
                { "table01_field02__1", "True" },
            };
            sd = factory.Serializer.Serialize(data);

            sut.StringValue = sd;
            ((DynamicDataTable)sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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

            sut.StringValue = data;
            var actual = factory.Serializer.Deserialize(sut.StringValue);

            actual.Should().NotBeNull();

            ((DynamicDataTable)sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [TestCase("this should be ignored")]
        [STAThread]
        [Apartment(ApartmentState.STA)]
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

            act.Should().NotThrow<Exception>();

            ((DynamicDataTable)sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
        [STAThread]
        public void Not_Support_direct_binding_or_validation_Right_Now()
        {
            var sut = new NZazuDataTableField(new FieldDefinition { Key = "table01" });
            sut.ContentProperty.Should().Be(null);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [STAThread]
        public void Create_ContentControl()
        {
            var sut = new NZazuDataTableField(new FieldDefinition { Key = "table01" });
            var contentControl = (ContentControl)sut.ValueControl;

            contentControl.Should().NotBeNull();

            contentControl.Focusable.Should().BeFalse("group fields should not have a tab stop of their own");
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Be_Editable_To_Make_Sure_GetSet_StringValue_Are_Called()
        {
            var sut = new NZazuDataTableField(new FieldDefinition { Key = "table01" });
            sut.IsEditable.Should().BeTrue();
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
        public void Handle_Add_Button()
        {
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "key",
                Type = "table01",
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = "table01_field01",
                        Type = "string"
                    }
                }
            })
            {
                FieldFactory = new NZazuFieldFactory()
            };

            var data = new Dictionary<string, string> { { "table01_field01__1", "hello" }, { "table01_field01__2", "world" } };
            var dataSerialized = sut.FieldFactory.Serializer.Serialize(data);
            sut.StringValue = dataSerialized;

            var ctrl = (DynamicDataTable)sut.ValueControl;
            var lastadded = ctrl.LayoutGrid.Children[2];
            lastadded.Should().NotBeNull();

            // lets see if it adds a row
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(3);
            sut.AddRowAbove(lastadded);
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(4);
        }
    }
}