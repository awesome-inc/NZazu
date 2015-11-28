using System;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof (ValueCheckResult))]
    // ReSharper disable InconsistentNaming
    internal class ValueCheckResult_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new ValueCheckResult(true);
            sut.IsValid.Should().BeTrue();
            sut.Error.Should().BeNull();

            var exception = new InvalidCastException();
            sut = new ValueCheckResult(false, exception);
            sut.IsValid.Should().BeFalse();
            sut.Error.Should().Be(exception);
        }

    }
}