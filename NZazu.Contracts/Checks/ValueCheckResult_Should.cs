using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using System;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(ValueCheckResult))]
    // ReSharper disable InconsistentNaming
    internal class ValueCheckResult_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new ValueCheckResult(true);
            sut.IsValid.Should().BeTrue();
            sut.Error.Should().BeNull();
        }

        [Test]
        public void Be_Creatable_With_Exception()
        {
            var exception = new InvalidCastException();
            var sut = new ValueCheckResult(false, exception);
            sut.IsValid.Should().BeFalse();
            sut.Error.Should().Be(exception);
        }

        [Test]
        public void Be_Creatable_With_Exception_Only()
        {
            var exception = new InvalidCastException();
            var sut = new ValueCheckResult(exception);
            sut.IsValid.Should().BeFalse();
            sut.Error.Should().Be(exception);
        }
    }
}