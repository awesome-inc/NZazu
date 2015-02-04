using System;
using System.Windows.Controls;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuField_Should
    {
        [Test]
        public void Validate_ctor_parameters()
        {
            new Action(() => new NZazuField("")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new NZazuField(null)).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new NZazuField("\t\r\n ")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Not_Create_Empty_Label()
        {
            var sut = new NZazuField("test");
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuField("test")
            {
                Prompt = "superhero"
            };

            var label = (Label)sut.LabelControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Prompt);
        }

        [Test]
        public void Create_ValueControl_Matching_Description()
        {
            var sut = new NZazuField("test")
            {
                Description = "superhero is alive"
            };

            var label = (Label)sut.ValueControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Description);
        }

        [Test]
        public void Not_Create_ValueControl_On_Empty_Description()
        {
            var sut = new NZazuField("test");

            var label = (Label)sut.ValueControl;
            label.Should().BeNull();
        }

        [Test]
        public void Get_Set_Value_should_do_nothing()
        {
            var sut = new NZazuField("test");
            sut.Value.Should().BeEmpty();

            sut.Value = "test";

            sut.Value.Should().BeEmpty();
        }

        /*
        [Test]
        public void IsValid_should_run_check()
        {
            const bool expected = false;
            var check = Substitute.For<IValueCheck>();
            check.Validate(Arg.Any<string>()).Returns(expected);

            var sut = new NZazuField("test") {Checks = new[] {check}};

            sut.IsValid.Should().Be(expected);

            check.Received().Validate(sut.Value);
        }



        [Test]
        public void IsValid_no_checks_should_return_true()
        {
            var sut = new NZazuField("test");
            sut.Checks.Should().BeNullOrEmpty();
            sut.IsValid.Should().BeTrue("no checks");
        }*/

        [Test]
        public void Attach_FieldValidationRule_according_to_checks()
        {
            var check = Substitute.For<IValueCheck>();
            check.When(x => x.Validate(Arg.Any<string>())).Do(x => { throw new ValidationException("test"); });

            var sut = new NZazuField("test") { Description="description", Checks = new[] { check } };

            var expectedRule = new CheckValidationRule(check);
            //sut.Binding.Should().NotBeNull();


            //var ()control = sut.ValueControl;
            //control.Should().NotBeNull();

            //control.GetBindingExpression()
        }
    }
}