using NEdifis.Attributes;
using NUnit.Framework;
using System.Linq;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(AggregateCheck))]
    // ReSharper disable InconsistentNaming
    internal class AggregateCheck_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new AggregateCheck(Enumerable.Empty<IValueCheck>());
        }

        //[Test]
        //public void Validate_should_run_all_checks()
        //{
        //    var check1 = Substitute.For<IValueCheck>();
        //    var check2 = Substitute.For<IValueCheck>();
        //    var sut = new AggregateCheck(check1, check2);

        //    const string input = "foobar";
        //    var formatProvider = CultureInfo.InvariantCulture;
        //    var error = new ValueCheckResult(true, new Exception("test"));
        //    var errorsValid = new AggregateValueCheckResult(Enumerable.Empty<ValueCheckResult>());
        //    var errors = new AggregateValueCheckResult(new[] { error });

        //    // true AND true => true
        //    check1.Validate(input, input, formatProvider).Returns(ValueCheckResult.Success);
        //    check2.Validate(input, input, formatProvider).Returns(ValueCheckResult.Success);
        //    sut.Validate(input, input, formatProvider);

        //    check1.Received().Validate(input, input, formatProvider);
        //    check2.Received().Validate(input, input, formatProvider);

        //    // false AND false => false
        //    check1.Validate(input, input, formatProvider).Returns(error);
        //    check2.Validate(input, input, formatProvider).Returns(error);

        //    var actual = sut.Validate(input, input, formatProvider);
        //    actual.Should().BeEquivalentTo(errorsValid);

        //    // false AND true => false
        //    check2.Validate(input, input, formatProvider).Returns(ValueCheckResult.Success);
        //    actual = sut.Validate(input, input, formatProvider);
        //    actual.Should().Be(error);

        //    // true AND false => false
        //    check1.Validate(input, input, formatProvider).Returns(ValueCheckResult.Success);
        //    check2.Validate(input, input, formatProvider).Returns(error);
        //    actual = sut.Validate(input, input, formatProvider);
        //    actual.Should().Be(error);
        //}
    }
}