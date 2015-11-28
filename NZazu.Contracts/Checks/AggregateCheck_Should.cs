using System.Globalization;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof (AggregateCheck))]
    // ReSharper disable InconsistentNaming
    internal class AggregateCheck_Should
    {
        [Test]
        public void Validate_should_run_all_checks()
        {
            var check1 = Substitute.For<IValueCheck>();
            var check2 = Substitute.For<IValueCheck>();
            var sut = new AggregateCheck(check1, check2);

            const string input = "foobar";
            var formatProvider = CultureInfo.InvariantCulture;
            var error = new ValueCheckResult(false, "test");

            // true AND true => true
            check1.Validate(input, formatProvider).Returns(ValueCheckResult.Success);
            check2.Validate(input, formatProvider).Returns(ValueCheckResult.Success);
            sut.Validate(input, formatProvider);

            check1.Received().Validate(input, formatProvider);
            check2.Received().Validate(input, formatProvider);

            // false AND false => false
            check1.Validate(input, formatProvider).Returns(error);
            check2.Validate(input, formatProvider).Returns(error);

            var actual = sut.Validate(input, formatProvider);
            actual.Should().Be(error);

            // false AND true => false
            check2.Validate(input, formatProvider).Returns(ValueCheckResult.Success);
            actual = sut.Validate(input, formatProvider);
            actual.Should().Be(error);

            // true AND false => false
            check1.Validate(input, formatProvider).Returns(ValueCheckResult.Success);
            check2.Validate(input, formatProvider).Returns(error);
            actual = sut.Validate(input, formatProvider);
            actual.Should().Be(error);
        }
    }
}