using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class StringRegExCheck_Should
    {
        [Test]
        public void IsValid_null_or_whitespace_should_pass()
        {
            var check = new StringRegExCheck("test", new Regex(@"^.*$", RegexOptions.IgnoreCase));

            check.Validate(null);
            check.Validate(String.Empty);
            check.Validate("\t\r\n");
            check.Validate(" ");
        }

        [Test]
        public void EmailChecks()
        {
            var check = new StringRegExCheck("Not a valid e-mail", new Regex(@"^.+@.+\..+$", RegexOptions.IgnoreCase));

            check.Validate("joe.doe@domain.com");
            Assert.Throws<ValidationException>(() => check.Validate("@domain.com")); // missing account prefix
            Assert.Throws<ValidationException>(() => check.Validate("joe.doe_domain.com")); // missing separator '@'
            Assert.Throws<ValidationException>(() => check.Validate("joe.doe@")); // missing domain
            Assert.Throws<ValidationException>(() => check.Validate("joe.doe@domain_de")); // missing domain separator '.'
        }

        [Test]
        public void IpAddressChecks()
        {
            var check = new StringRegExCheck("Not a valid ip.", new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"));

            check.Validate("1.1.1.1");
            check.Validate("22.22.22.22");
            check.Validate("333.333.333.333"); // actually this is not a valid IP address. However, we check the RegEx here!

            Assert.Throws<ValidationException>(() => check.Validate("1.1.11")); // missing separator
            Assert.Throws<ValidationException>(() => check.Validate("1.22.33.4444")); // field with 4 digits
        }

        [Test]
        public void Validate_should_OR_all_regexes()
        {
            var twoChars = new Regex("[a-b]{2}");
            var twoDigits = new Regex(@"\d{2}");
            var check = new StringRegExCheck("Enter 2 chars or 2 digits", twoChars, twoDigits);

            Assert.Throws<ValidationException>(() => check.Validate("a"));

            // false OR false => false
            Assert.Throws<ValidationException>(() => check.Validate("a"));

            // false OR true => true
            check.Validate("12");

            // true OR false => true
            check.Validate("ab");
        }
    }
}