using System.Collections.Generic;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Extensions
{
    [TestFixtureFor(typeof(ViewExtensions))]
    // ReSharper disable InconsistentNaming
    internal class ViewExtensions_Should
    {
        [Test]
        public void Set_field_values_on_SetFieldValues()
        {
            var view = Substitute.For<INZazuWpfView>();

            const string key = "name";
            const string value = "John";

            var formDefinition = new FormDefinition { Fields = new[] { new FieldDefinition { Key = key } } };
            view.FormDefinition = formDefinition;

            var field = Substitute.For<INZazuWpfField>();
            view.GetField(key).Returns(field);

            var input = new Dictionary<string, string> { { key, value } };

            view.SetFieldValues(input);

            field.StringValue.Should().Be(value);
        }

        [Test]
        public void Ignore_missing_field_on_SetFieldValues()
        {
            var view = Substitute.For<INZazuWpfView>();

            const string key = "name";
            const string value = "John";

            var formDefinition = new FormDefinition { Fields = new[] { new FieldDefinition { Key = key } } };
            view.FormDefinition = formDefinition;

            var field = Substitute.For<INZazuWpfField>();
            view.GetField(key).Returns(field);

            var input = new Dictionary<string, string>
            {
                { "another key", "somevalue" },
                {key, value}
            };

            view.SetFieldValues(input);

            field.StringValue.Should().Be(value, because: "nothing can be changed");
        }

        [Test]
        public void Return_False_If_Validate_Has_Exception()
        {
            var view = Substitute.For<INZazuWpfView>();
            view.Validate().Returns(new ValueCheckResult(false, "I am invalid"));

            view.IsValid().Should().BeFalse();

            view.ReceivedWithAnyArgs().Validate();
        }

        [Test]
        public void Return_True_If_Validate()
        {
            var view = Substitute.For<INZazuWpfView>();
            view.Validate().Returns(ValueCheckResult.Success);

            view.IsValid().Should().BeTrue();

            view.ReceivedWithAnyArgs().Validate();
        }
    }
}