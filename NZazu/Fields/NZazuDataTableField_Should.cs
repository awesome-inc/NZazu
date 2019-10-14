using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;
using NZazu.Fields.Controls;
using NZazu.Serializer;

#pragma warning disable 618

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuDataTableField))]
    // ReSharper disable InconsistentNaming
    internal class NZazuDataTableField_Should
    {
        private static object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            if (type == typeof(INZazuTableDataSerializer)) return new NZazuTableDataXmlSerializer();
            if (type == typeof(INZazuWpfFieldFactory)) return new NZazuFieldFactory();
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition {Key = "table01_field01", Type = "string"},
                    new FieldDefinition {Key = "table01_field02", Type = "bool"}
                }
            }, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Be_Disposable()
        {
            using (var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition {Key = "table01_field01", Type = "string"},
                    new FieldDefinition {Key = "table01_field02", Type = "bool"}
                }
            }, ServiceLocator))
            {
                sut.AddRowAbove(sut.ValueControl);
            }
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
            }, ServiceLocator);

            // create factory later but before "ValueControl" access
            var _factory = new NZazuFieldFactory();

            // now lets create some testdata and return it for multiple tests
            var data = new Dictionary<string, string>
            {
                {"table01_field01__1", "Hello"},
                {"table01_field02__1", "True"},
                {"table01_field01__2", "World"},
                {"table01_field02__2", "False"},
                {"table01_field01__3", "42"},
                {"table01_field02__3", ""}
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
            var serializer = new NZazuTableDataXmlSerializer();
            var testData = GetField();
            var sut = (NZazuDataTableField) testData.Field;
            var data = (Dictionary<string, string>) testData.Data;

            // lets assign the data and do some tests
            sut.SetValue(serializer.Serialize(data));
            var actual = serializer.Deserialize(sut.GetValue());
            foreach (var dta in data)
                actual.Should().Contain(dta);

            sut.Validate().IsValid.Should().BeTrue();
            ((DynamicDataTable) sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(4);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Serialize_And_Deserialize_Multiple()
        {
            var serializer = new NZazuTableDataXmlSerializer();
            var testData = GetField();
            var sut = (NZazuDataTableField) testData.Field;
            var data = (Dictionary<string, string>) testData.Data;
            var sd = serializer.Serialize(data);

            // lets assign the data
            sut.SetValue(sd);

            // lets assign the data again
            var testData2 = GetField();
            var sut2 = (NZazuDataTableField) testData2.Field;
            sut2.SetValue(sd);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Assign_Data_Multiple_Times()
        {
            var serializer = new NZazuTableDataXmlSerializer();
            var testData = GetField();
            var sut = (NZazuDataTableField) testData.Field;
            var data = (Dictionary<string, string>) testData.Data;
            var sd = serializer.Serialize(data);

            // lets assign the data
            sut.SetValue(sd);
            ((DynamicDataTable) sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(4);

            // lets assign other data
            data = new Dictionary<string, string>
            {
                {"table01_field01__1", "Hello"},
                {"table01_field02__1", "True"}
            };
            sd = serializer.Serialize(data);

            sut.SetValue(sd);
            ((DynamicDataTable) sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Serialize_And_Deserialize_Null_Rows_To_Null()
        {
            var serializer = new NZazuTableDataXmlSerializer();
            var data = new Dictionary<string, string> {{"table01_field01__1", null}, {"table01_field01__2", null}};
            var dataSerialized = serializer.Serialize(data);

            // ReSharper disable once UseObjectOrCollectionInitializer
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition {Key = "table01_field01", Type = "string"}
                }
            }, ServiceLocator);

            sut.SetValue(dataSerialized);

            // test the returned data
            var actual = serializer.Deserialize(sut.GetValue());
            actual.Count.Should().Be(0);

            sut.Validate().IsValid.Should().BeTrue();
            ((DynamicDataTable) sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("<items/>")]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Deserialize_Empty_Data(string data)
        {
            object Fct(Type t)
            {
                if (t == typeof(INZazuTableDataSerializer)) return new NZazuTableDataXmlSerializer();
                if (t == typeof(INZazuWpfFieldFactory)) return new NZazuFieldFactory();
                return null;
            }

            // ReSharper disable once UseObjectOrCollectionInitializer
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition {Key = "table01_field01", Type = "string"}
                }
            }, ServiceLocator);

            sut.SetValue(data);
            var actual =
                ((INZazuTableDataSerializer) Fct(typeof(INZazuTableDataSerializer))).Deserialize(sut.GetValue());

            actual.Should().NotBeNull();

            ((DynamicDataTable) sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [TestCase("this should be ignored")]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Ignore_Exception_On_Deserialize_Invalid_Data(string data)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition {Key = "table01_field01", Type = "string"}
                }
            }, ServiceLocator);

            Action act = () => { sut.SetValue(data); };

            act.Should().NotThrow<Exception>();

            ((DynamicDataTable) sut.ValueControl).LayoutGrid.RowDefinitions.Count.Should().Be(2);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Validate()
        {
            var s = new NZazuTableDataXmlSerializer();
            var data = new Dictionary<string, string> {{"table01_field01__1", ""}, {"table01_field01__2", "world"}};
            var dataSerialized = s.Serialize(data);

            // ReSharper disable once UseObjectOrCollectionInitializer
            var sut = new NZazuDataTableField(new FieldDefinition
            {
                Key = "table01",
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = "table01_field01", Type = "string",
                        Checks = new[] {new CheckDefinition {Type = "required"}}
                    }
                }
            }, ServiceLocator);

            // ReSharper disable once UnusedVariable
            var justToMakeTheCall = sut.ValueControl;

            sut.SetValue(dataSerialized);
            sut.Validate().IsValid.Should().BeFalse();

            // now change the data
            data = new Dictionary<string, string> {{"table01_field01__1", "hello"}, {"table01_field01__2", "world"}};
            dataSerialized = s.Serialize(data);
            sut.SetValue(dataSerialized);
            sut.Validate().IsValid.Should().BeTrue();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [STAThread]
        public void Not_Support_direct_binding_or_validation_Right_Now()
        {
            var sut = new NZazuDataTableField(new FieldDefinition {Key = "table01"}, ServiceLocator);
            sut.ContentProperty.Should().Be(null);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [STAThread]
        public void Create_ContentControl()
        {
            var sut = new NZazuDataTableField(new FieldDefinition {Key = "table01"}, ServiceLocator);
            var contentControl = (ContentControl) sut.ValueControl;

            contentControl.Should().NotBeNull();

            contentControl.Focusable.Should().BeFalse("group fields should not have a tab stop of their own");
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Be_Editable_To_Make_Sure_GetSet_StringValue_Are_Called()
        {
            var sut = new NZazuDataTableField(new FieldDefinition {Key = "table01"}, ServiceLocator);
            sut.IsEditable.Should().BeTrue();
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Handle_Delete_Button()
        {
            var serializer = new NZazuTableDataXmlSerializer();
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
            }, ServiceLocator);

            var ctrl = (DynamicDataTable) sut.ValueControl;

            var data = new Dictionary<string, string>
                {{"table01_field01__1", "hello"}, {"table01_field01__2", "world"}};
            var dataSerialized = serializer.Serialize(data);
            sut.SetValue(dataSerialized);

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
            var serializer = new NZazuTableDataXmlSerializer();
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
            }, ServiceLocator);

            var data = new Dictionary<string, string>
                {{"table01_field01__1", "hello"}, {"table01_field01__2", "world"}};
            var dataSerialized = serializer.Serialize(data);
            sut.SetValue(dataSerialized);

            var ctrl = (DynamicDataTable) sut.ValueControl;
            var lastadded = ctrl.LayoutGrid.Children[2];
            lastadded.Should().NotBeNull();

            // lets see if it adds a row
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(3);
            sut.AddRowAbove(lastadded);
            ctrl.LayoutGrid.RowDefinitions.Count.Should().Be(4);
        }
    }
}