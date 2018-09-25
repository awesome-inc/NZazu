using System;
using System.Text.RegularExpressions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(StringRegExCheck))]
    // ReSharper disable InconsistentNaming
    internal class StringRegExCheck_Should
    {
        [Test]
        public void IsValid_null_or_whitespace_should_pass()
        {
            var check = new StringRegExCheck("test", new Regex(@"^.*$", RegexOptions.IgnoreCase));

            check.ShouldPass(null, null);
            check.ShouldPass(string.Empty, string.Empty);
            check.ShouldPass("\t\r\n", "\t\r\n");
            check.ShouldPass(" ", " ");
        }

        [Test]
        public void EmailChecks()
        {
            var check = new StringRegExCheck("Not a valid e-mail", new Regex(@"^.+@.+\..+$", RegexOptions.IgnoreCase));

            check.ShouldPass("joe.doe@domain.com", "joe.doe@domain.com");
            check.ShouldFailWith<ArgumentException>("@domain.com", null); // missing account prefix
            check.ShouldFailWith<ArgumentException>("joe.doe_domain.com", null); // missing separator '@'
            check.ShouldFailWith<ArgumentException>("joe.doe@", null); // missing domain
            check.ShouldFailWith<ArgumentException>("joe.doe@domain_de", null); // missing domain separator '.'
        }

        [Test]
        public void IpAddressChecks()
        {
            var check = new StringRegExCheck("Not a valid ip.", new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"));

            check.ShouldPass("1.1.1.1", "1.1.1.1");
            check.ShouldPass("22.22.22.22", "22.22.22.22");
            check.ShouldPass("333.333.333.333", "333.333.333.333"); // actually this is not a valid IP address. However, we check the RegEx here!

            check.ShouldFailWith<ArgumentException>("1.1.11", 0); // missing separator
            check.ShouldFailWith<ArgumentException>("1.22.33.4444", 0); // field with 4 digits
        }

        [Test]
        public void Validate_should_OR_all_regexes()
        {
            var twoChars = new Regex("[a-b]{2}");
            var twoDigits = new Regex(@"\d{2}");
            var check = new StringRegExCheck("Enter 2 chars or 2 digits", twoChars, twoDigits);

            check.ShouldFailWith<ArgumentException>("a", "a");

            // false OR false => false
            check.ShouldFailWith<ArgumentException>("a", "a");

            // false OR true => true
            check.ShouldPass("12", "12");

            // true OR false => true
            check.ShouldPass("ab", "ab");
        }
    }
}