using System;
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
            check.Validate(input, cultureInfo).Returns(ValueCheckResult.Success);
            var result = sut.Validate(input, cultureInfo);
            result.IsValid.Should().BeTrue();
            result.ErrorContent.Should().BeNull();

            // validation is false
            var error = new ValueCheckResult(false, "test");
            check.Validate(input, cultureInfo).Returns(error);
            result = sut.Validate(input, cultureInfo);
            result.IsValid.Should().BeFalse();
            result.ErrorContent.Should().Be(error.Error);

            var exception = new ArgumentException("test");
            error = new ValueCheckResult(false, exception);
            check.Validate(input, cultureInfo).Returns(error);
            result = sut.Validate(input, cultureInfo);
            result.IsValid.Should().BeFalse();
            result.ErrorContent.Should().Be(exception.Message);
        }
    }
}