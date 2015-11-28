using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuFieldFactory_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuFieldFactory();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfFieldFactory>();
        }

        [Test]
        [TestCase(null, typeof(Label))]
        [TestCase("label", typeof(Label))]
        [TestCase("string", typeof(TextBox))]
        [TestCase("bool", typeof(CheckBox))]
        [TestCase("int", typeof(TextBox))]
        [TestCase("date", typeof(DatePicker))]
        [TestCase("double", typeof(TextBox))]
        [TestCase("group", typeof(ContentControl))]
        [TestCase("option", typeof(ComboBox))]
        [STAThread]
        public void Support(string fieldType, Type controlType)
        {
            var sut = new NZazuFieldFactory();

            var field = sut.CreateField(new FieldDefinition { Key = "test", Type = fieldType, Description = "test" });

            field.Should().NotBeNull();
            field.Type.Should().Be(fieldType ?? "label"); // because of the fallback in case of null

            var control = field.ValueControl;
            control.Should().BeOfType(controlType);
        }

        [Test]
        [STAThread]
        public void Return_Label_If_Not_Supported()
        {
            var sut = new NZazuFieldFactory();

            var field =
                sut.CreateField(new FieldDefinition
                {
                    Key = "test",
                    Type = "I am a not supported label",
                    Description = "test"
                });

            field.Should().NotBeNull();
            field.Type.Should().Be("label", because: "the fallback is label");
            // because of the fallback in case of null

            var control = field.ValueControl;
            control.Should().BeOfType<Label>();
        }

        [Test]
        public void Use_CheckFactory_for_creating_checks()
        {
            var checkFactory = Substitute.For<ICheckFactory>();
            var checkDefinition = new CheckDefinition { Type = "required" };
            var check = new RequiredCheck();
            checkFactory.CreateCheck(checkDefinition).Returns(check);

            var sut = new NZazuFieldFactory(checkFactory);
            var fieldDefinition = new FieldDefinition { Key = "test", Type = "string", Checks = new[] { checkDefinition } };
            var field = (NZazuField)sut.CreateField(fieldDefinition);

            field.Should().NotBeNull();
            field.Check.Should().Be(check);
        }

        [Test]
        public void Use_AggregateCheck_for_multiple_checks()
        {
            var checkFactory = Substitute.For<ICheckFactory>();
            var checkDefinition1 = new CheckDefinition { Type = "required" };
            var checkDefinition2 = new CheckDefinition { Type = "length", Values = new[] { "4", "6" } };
            var check1 = new RequiredCheck();
            var check2 = new StringLengthCheck(4, 6);
            checkFactory.CreateCheck(checkDefinition1).Returns(check1);
            checkFactory.CreateCheck(checkDefinition2).Returns(check2);

            var sut = new NZazuFieldFactory(checkFactory);

            var fieldDefinition = new FieldDefinition
            {
                Key = "test",
                Type = "string",
                Checks = new[] { checkDefinition1, checkDefinition2 }
            };

            var field = (NZazuField)sut.CreateField(fieldDefinition);

            var aggregateCheck = (AggregateCheck)field.Check;
            aggregateCheck.Should().NotBeNull();
            aggregateCheck.Checks.ShouldBeEquivalentTo(new IValueCheck[] { check1, check2 });
        }

        [Test]
        public void Copy_Settings_from_FieldDefinition()
        {
            var sut = new NZazuFieldFactory();
            var fieldDefinition = new FieldDefinition { Key = "test", Type = "string" };
            var field = (NZazuField)sut.CreateField(fieldDefinition);
            field.Settings.Should().NotBeNull();
            field.Settings.Should().BeEmpty();

            var settings = new Dictionary<string, string> { { "key", "value" } };
            fieldDefinition.Settings = settings;

            field = (NZazuField)sut.CreateField(fieldDefinition);
            field.Settings.ShouldBeEquivalentTo(settings);
        }

        [Test]
        public void Does_Not_Attach_Behavior_To_Field()
        {
            // because view has the resolver and attaches the behavior
            var sut = new NZazuFieldFactory();
            var field = sut.CreateField(
                new FieldDefinition
                {
                    Key = "test",
                    Type = "string",
                    Behavior = new BehaviorDefinition { Name = "Empty" }
                });
            field.Should().NotBeNull();
            field.Behavior.Should().BeNull();
        }

        [Test]
        public void Recursively_create_GroupFields()
        {
            var sut = new NZazuFieldFactory();

            var fields = new[]
            {
                new FieldDefinition
                {
                    Key = "first",
                    Type = "string", 
                },
                new FieldDefinition
                {
                    Key = "second",
                    Type = "string", 
                }
            };

            var fieldDefinition = new FieldDefinition
            {
                Key = "group1", Type = "group",
                Fields = fields
            };
            var field = (INZazuWpfGroupField)sut.CreateField(fieldDefinition);

            field.Should().NotBeNull();

            field.Fields.Should().HaveCount(fieldDefinition.Fields.Length);
        }

        [Test]
        public void Copy_group_layout()
        {
            var sut = new NZazuFieldFactory();

            var fieldDefinition = new FieldDefinition
            {
                Key = "group1",
                Type = "group",
                Layout = "grid"
            };
            var field = (INZazuWpfGroupField)sut.CreateField(fieldDefinition);

            field.Layout.Should().Be(fieldDefinition.Layout);
        }

        [Test]
        public void Copy_values_for_option_field()
        {
            var sut = new NZazuFieldFactory();

            var fieldDefinition = new FieldDefinition
            {
                Key = "test", Type = "option",
                Values = new[] { "1","2","3"}
            };

            var field = (NZazuOptionsField)sut.CreateField(fieldDefinition);

            field.Options.Should().BeEquivalentTo(fieldDefinition.Values);
        }
    }
}