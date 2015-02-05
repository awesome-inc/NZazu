using System;
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
    class NZazuFieldFactory_Should
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
        //[TestCase("birthday", "dateTime", "Birthday", "Choose birthday", "Your birthday", typeof (DatePicker))]
        //[TestCase("weight", "double", "Weight", "Enter body weight (kg)", "Your weight", typeof(TextBox))]
        public void Support(string fieldType, Type controlType)
        {
            var sut = new NZazuFieldFactory();

            var field = sut.CreateField(new FieldDefinition { Key = "test", Type = fieldType, Description = "test"});

            field.Should().NotBeNull();
            field.Type.Should().Be(fieldType ?? "label"); // because of the fallback in case of null

            var control = field.ValueControl;
            control.Should().BeOfType(controlType);
        }

        [Test]
        public void Set_Checks()
        {
            var sut = new NZazuFieldFactory();
            var check = Substitute.For<IValueCheck>();
            var checks = new[] { check };
            var field = (NZazuField)sut.CreateField(new FieldDefinition { Key = "test", Type = "string", Checks = checks });

            field.Should().NotBeNull();
            field.Checks.Should().BeSameAs(checks);
        }
    }
}