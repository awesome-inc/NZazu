using System.Globalization;
using NSubstitute;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class AggregateCheck_Should
    {
        [Test]
        public void Validate_should_run_all_checks()
        {
            var check1 = Substitute.For<IValueCheck>();
            var check2 = Substitute.For<IValueCheck>();
            var sut = new AggregateCheck(check1, check2);

            const string input = "foobar";
            var formatProvider = CultureInfo.InvariantCulture;
            var error = new ValidationException("test");

            // true AND true => true
            sut.Validate(input, formatProvider);

            check1.Received().Validate(input, formatProvider);
            check2.Received().Validate(input, formatProvider);

            // false AND false => false
            check1.When(x => x.Validate(input, formatProvider)).Do(x => { throw error; });
            check2.When(x => x.Validate(input, formatProvider)).Do(x => { throw error; });

            Assert.Throws<ValidationException>(() => sut.Validate(input, formatProvider));

            // false AND true => false
            check2.When(x => x.Validate(input, formatProvider)).Do(x => { });
            Assert.Throws<ValidationException>(() => sut.Validate(input, formatProvider));

            // true AND false => false
            check1.When(x => x.Validate(input, formatProvider)).Do(x => { });
            check2.When(x => x.Validate(input, formatProvider)).Do(x => { throw error; });
            Assert.Throws<ValidationException>(() => sut.Validate(input, formatProvider));
        }
    }
}