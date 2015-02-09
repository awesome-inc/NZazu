using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Fields;

namespace NZazu.Factory
{
    [TestFixture, RequiresSTA]
    // ReSharper disable InconsistentNaming
    internal class NZazuFieldFactory_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuFieldFactory();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuFieldFactory>();
        }

        [Test]
        [TestCase(null, typeof(Label))]
        [TestCase("label", typeof(Label))]
        [TestCase("string", typeof(TextBox))]
        [TestCase("bool", typeof(CheckBox))]
        [TestCase("int", typeof(TextBox))]
        [TestCase("date", typeof(DatePicker))]
        [TestCase("double", typeof(TextBox))]
        [TestCase("richtext", typeof(RichTextBox))]
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
        public void Atach_Behavior_To_Field()
        {
            var sut = new NZazuFieldFactory();

            var field = sut.CreateField(
                new FieldDefinition
                {
                    Key = "test",
                    Type = "string", 
                    Behavior = new BehaviorDefinition { Name = "empty" }
                });
            field.Should().NotBeNull();

            var control = field.ValueControl;
        }

    }
}