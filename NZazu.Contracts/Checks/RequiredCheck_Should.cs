using System;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class RequiredCheck_Should
    {
        [Test]
        public void Throw_ValidationException_if_value_null_or_whitespace()
        {
            var check = new RequiredCheck();

            Assert.Throws<ValidationException>(() => check.Validate(null));
            Assert.Throws<ValidationException>(() => check.Validate(String.Empty));
            Assert.Throws<ValidationException>(() => check.Validate("\t\r\n"));

            Assert.Throws<ValidationException>(() => check.Validate(" "));

            check.Validate("a");
        }
    }
}