using System;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    internal class RequiredCheck_Should
    {
        [Test]
        public void Throw_ValidationException_if_value_null_or_whitespace()
        {
            var check = new RequiredCheck();

            check.ShouldFailWith<ArgumentException>(null);
            check.ShouldFailWith<ArgumentException>(string.Empty);
            check.ShouldFailWith<ArgumentException>("\t\r\n");
            check.ShouldFailWith<ArgumentException>(" ");

            check.Validate("a").IsValid.Should().BeTrue();
        }
    }
}