using System;
using System.Linq;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;

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

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<IValueCheck>();
        }

        [Test]
        public void Validate_Should_Accept_Null_Results_As_Success()
        {
            var check1 = Substitute.For<IValueCheck>();
            var check2 = Substitute.For<IValueCheck>();
            check1.Validate(null, null, null).Returns((ValueCheckResult) null);
            check2.Validate(null, null, null).Returns((ValueCheckResult) null);

            var sut = new AggregateCheck(new[] {check1, check2});
            var result = sut.Validate(null, null, null);

            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.Exception.Should().BeNull();
        }

        [Test]
        public void Validate_should_run_all_checks()
        {
            var check1 = Substitute.For<IValueCheck>();
            var check2 = Substitute.For<IValueCheck>();
            check1.Validate(null, null, null).Returns((ValueCheckResult) null);
            check2.Validate(null, null, null).Returns((ValueCheckResult) null);

            var sut = new AggregateCheck(new[] {check1, check2});
            var result = sut.Validate(null, null, null);

            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.Exception.Should().BeNull();

            check1.Received(1).Validate(null, null, null);
            check2.Received(1).Validate(null, null, null);
        }

        [Test]
        public void Aggregate_Single_Error_Results()
        {
            var exception = new Exception();
            var checkResult1 = new ValueCheckResult(exception);
            var checkResult2 = new ValueCheckResult(true);
            var check1 = Substitute.For<IValueCheck>();
            var check2 = Substitute.For<IValueCheck>();
            check1.Validate(null, null, null).Returns(checkResult1);
            check2.Validate(null, null, null).Returns(checkResult2);

            var sut = new AggregateCheck(new[] {check1, check2});
            var result = sut.Validate(null, null, null);

            result.Should().NotBeNull();
            result.Should().Be(checkResult1, "we return the one and only error");
            result.IsValid.Should().BeFalse();
            result.Exception.Should().Be(exception);
        }

        [Test]
        public void Aggregate_Multiple_Error_Results()
        {
            var exception1 = new Exception();
            var exception2 = new Exception();
            var check1 = Substitute.For<IValueCheck>();
            var check2 = Substitute.For<IValueCheck>();
            check1.Validate(null, null, null).Returns(new ValueCheckResult(exception1));
            check2.Validate(null, null, null).Returns(new ValueCheckResult(exception2));

            var sut = new AggregateCheck(new[] {check1, check2});
            var result = sut.Validate(null, null, null);

            result.Should().NotBeNull();
            result.Should().BeOfType<AggregateValueCheckResult>("we return an aggregation of all errors");
            result.IsValid.Should().BeFalse();
            result.Exception.Should().BeOfType<AggregateException>();
        }

        //const string input = "foobar";
        //var formatProvider = CultureInfo.InvariantCulture;
        //var error = new ValueCheckResult(true, new Exception("test"));
        //var errorsValid = new AggregateValueCheckResult(Enumerable.Empty<ValueCheckResult>());
        //var errors = new AggregateValueCheckResult(new[] { error });

        //// true AND true => true
        //check1.Validate(input, input, formatProvider).Returns(ValueCheckResult.Success);
        //check2.Validate(input, input, formatProvider).Returns(ValueCheckResult.Success);
        //sut.Validate(input, input, formatProvider);

        //check1.Received().Validate(input, input, formatProvider);
        //check2.Received().Validate(input, input, formatProvider);

        //// false AND false => false
        //check1.Validate(input, input, formatProvider).Returns(error);
        //check2.Validate(input, input, formatProvider).Returns(error);

        //var actual = sut.Validate(input, input, formatProvider);
        //actual.Should().BeEquivalentTo(errorsValid);

        //// false AND true => false
        //check2.Validate(input, input, formatProvider).Returns(ValueCheckResult.Success);
        //actual = sut.Validate(input, input, formatProvider);
        //actual.Should().Be(error);

        //// true AND false => false
        //check1.Validate(input, input, formatProvider).Returns(ValueCheckResult.Success);
        //check2.Validate(input, input, formatProvider).Returns(error);
        //actual = sut.Validate(input, input, formatProvider);
        //actual.Should().Be(error);
        //}
    }
}