using System.Globalization;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts.Checks;

namespace NZazu
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class CheckValidationRule_Should
    {
        [Test]
        public void Delegate_validation_to_check()
        {
            var check = Substitute.For<IValueCheck>();

            var sut = new CheckValidationRule(check);

            // validation is true
            const string input = "foobar";
            var cultureInfo = CultureInfo.InvariantCulture;
            var result = sut.Validate(input, cultureInfo);
            result.IsValid.Should().BeTrue();
            result.ErrorContent.Should().BeNull();

            // validation is false
            var error = new ValidationException("test");
            check.When(x => x.Validate(input, cultureInfo)).Do(x => { throw error;});
            result = sut.Validate(input, cultureInfo);
            result.IsValid.Should().BeFalse();
            result.ErrorContent.Should().Be(error.Message);
        }
    }
}