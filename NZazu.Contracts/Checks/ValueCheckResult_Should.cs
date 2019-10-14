using System;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

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
            sut.Exception.Should().BeNull();
        }

        [Test]
        public void Be_Creatable_With_Exception()
        {
            var exception = new InvalidCastException();
            var sut = new ValueCheckResult(false, exception);
            sut.IsValid.Should().BeFalse();
            sut.Exception.Should().Be(exception);
        }

        [Test]
        public void Be_Creatable_With_Exception_Only()
        {
            var exception = new InvalidCastException();
            var sut = new ValueCheckResult(exception);
            sut.IsValid.Should().BeFalse();
            sut.Exception.Should().Be(exception);
        }
    }
}