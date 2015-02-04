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
            var cultureInfo = CultureInfo.InvariantCulture;
            var error = new ValidationException("test");

            // true AND true => true
            sut.Validate(input, cultureInfo);

            check1.Received().Validate(input, cultureInfo);
            check2.Received().Validate(input, cultureInfo);

            // false AND false => false
            check1.When(x => x.Validate(input, cultureInfo)).Do(x => { throw error; });
            check2.When(x => x.Validate(input, cultureInfo)).Do(x => { throw error; });

            Assert.Throws<ValidationException>(() => sut.Validate(input, cultureInfo));

            // false AND true => false
            check2.When(x => x.Validate(input, cultureInfo)).Do(x => { });
            Assert.Throws<ValidationException>(() => sut.Validate(input, cultureInfo));

            // true AND false => false
            check1.When(x => x.Validate(input, cultureInfo)).Do(x => { });
            check2.When(x => x.Validate(input, cultureInfo)).Do(x => { throw error; });
            Assert.Throws<ValidationException>(() => sut.Validate(input, cultureInfo));
        }
    }
}