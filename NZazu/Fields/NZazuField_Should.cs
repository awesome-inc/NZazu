using System;
using System.Linq;
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

        [Test]
        public void Not_Attach_FieldValidationRule_if_no_ContentProperty_set()
        {
            var check = Substitute.For<IValueCheck>();
            check.When(x => x.Validate(Arg.Any<string>())).Do(x => { throw new ValidationException("test"); });

            var sut = new NZazuField("test") { Description = "description", Checks = new[] { check } };
            sut.ContentProperty.Should().BeNull();
            sut.ValueControl.Should().NotBeNull();
        }

        [Test]
        public void Attach_FieldValidationRule_according_to_checks()
        {
            // but we need a dummy content enabled field -> no content, no validation
            var check = Substitute.For<IValueCheck>();
            check.When(x => x.Validate(Arg.Any<string>())).Do(x => { throw new ValidationException("test"); });

            var sut = new NZazuField_With_Description_As_Content_Property("test") { Description = "description", Checks = new[] { check } };

            var expectedRule = new CheckValidationRule(check);
            sut.ValueControl.Should().NotBeNull();

            var expression = sut.ValueControl.GetBindingExpression(sut.ContentProperty);
            expression.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
            var binding = expression.ParentBinding;
            binding.Should().NotBeNull();

            binding.ValidationRules.Single().ShouldBeEquivalentTo(expectedRule);
        }

        #region test NZazuField with bi-directional content property

        [Test]
        public void Get_Set_Value_should_set_content_if_content_property_set()
        {
            var sut = new NZazuField_With_Description_As_Content_Property("test") { Description = "hello there" };
            sut.Value.Should().Be("hello there");

            sut.Value = "test";

            sut.Value.Should().Be("test");
        }


        private class NZazuField_With_Description_As_Content_Property : NZazuField
        {
            public NZazuField_With_Description_As_Content_Property(string key)
                : base(key)
            {
                ContentProperty = ContentControl.ContentProperty;
            }
        }

        #endregion
    }
}