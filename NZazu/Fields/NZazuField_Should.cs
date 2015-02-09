using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        #region Test fields to verify base class

        [ExcludeFromCodeCoverage]
        private class NZazuDummyField : NZazuField
        {
            public NZazuDummyField(string key)
                : base(key)
            {
            }

            public override string StringValue { get; set; }
            public override string Type { get { return null; } }
            public override DependencyProperty ContentProperty { get { return null; } }
            protected override Control GetValue() { return null; }
        }

        private class NZazuField_With_Description_As_Content_Property : NZazuDummyField
        {
            public NZazuField_With_Description_As_Content_Property(string key)
                : base(key)
            {
            }

            public override DependencyProperty ContentProperty { get { return ContentControl.ContentProperty; } }
            protected override Control GetValue() { return new ContentControl(); }
        }

        #endregion

        [Test]
        public void Validate_ctor_parameters()
        {
            new Action(() => new NZazuDummyField("")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new NZazuDummyField(null)).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new NZazuDummyField("\t\r\n ")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Not_Create_Label_if_no_prompt()
        {
            var sut = new NZazuDummyField("test");
            sut.Prompt.Should().BeNullOrWhiteSpace();
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuDummyField("test")
            {
                Prompt = "superhero"
            };

            var label = (Label)sut.LabelControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Prompt);
        }

        [Test]
        public void Set_And_Get_Value()
        {
            var sut = new NZazuDummyField("test");
            sut.StringValue.Should().BeNull();

            sut.StringValue = "test";

            sut.StringValue.Should().Be("test");
        }

        [Test]
        public void Pass_Validation_To_Checks()
        {
            var check = Substitute.For<IValueCheck>();
            var sut = new NZazuDummyField("test") { Description = "description", Check = check };
            sut.Validate();

            check.ReceivedWithAnyArgs().Validate(Arg.Any<string>());
        }

        [Test]
        public void Pass_Validation_To_Checks_And_Rethrow_Exception()
        {
            var check = Substitute.For<IValueCheck>();
            check.When(x => x.Validate(Arg.Any<string>(), Arg.Any<IFormatProvider>())).Do(x => { throw new ValidationException("test"); });

            var sut = new NZazuDummyField("test") { Description = "description", Check =  check };
            new Action(sut.Validate).Invoking(a => a()).ShouldThrow<ValidationException>();
            check.ReceivedWithAnyArgs().Validate(Arg.Any<string>());
        }

        #region test NZazuDummyField with bi-directional content property

        [Test]
        public void Attach_FieldValidationRule_according_to_checks()
        {
            // but we need a dummy content enabled field -> no content, no validation
            var check = Substitute.For<IValueCheck>();
            check.When(x => x.Validate(Arg.Any<string>())).Do(x => { throw new ValidationException("test"); });

            var sut = new NZazuField_With_Description_As_Content_Property("test") 
                { Description = "description", Check = check };

            var expectedRule = new CheckValidationRule(check)
            {
                // we make sure the validation is executed on init
                ValidatesOnTargetUpdated = true
            };
            sut.ValueControl.Should().NotBeNull();

            var expression = sut.ValueControl.GetBindingExpression(sut.ContentProperty);
            expression.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
            var binding = expression.ParentBinding;
            binding.Should().NotBeNull();
            binding.Source.Should().Be(sut, because: "we need a source for data binding");
            binding.Mode.Should().Be(BindingMode.TwoWay, because: "we update databinding in two directions");
            binding.UpdateSourceTrigger
                .Should().Be(UpdateSourceTrigger.PropertyChanged, because: "we want validation during edit");

            binding.ValidationRules.Single().ShouldBeEquivalentTo(expectedRule);
        }

        #endregion
    }
}