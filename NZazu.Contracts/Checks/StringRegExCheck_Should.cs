using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(StringRegExCheck))]
    // ReSharper disable InconsistentNaming
    internal class StringRegExCheck_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var settings =
                new Dictionary<string, string>
                    {{"Hint", "this is the hint"}, {"RegEx", "false|true"}} as IDictionary<string, string>;
            var ctx = new ContextFor<StringRegExCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            sut.Should().NotBeNull();
            sut.GetType().GetCustomAttribute<DisplayNameAttribute>().DisplayName.Should().Be("regex");
        }

        [Test]
        public void Registered_At_CheckFactory()
        {
            var settings =
                new Dictionary<string, string>
                    {{"Hint", "this is the hint"}, {"RegEx", "false|true"}} as IDictionary<string, string>;
            var checkType = typeof(StringRegExCheck);

            var sut = new CheckFactory();

            var checkDefinition = new CheckDefinition {Type = "regex", Settings = settings};
            var check = sut.CreateCheck(checkDefinition, new FieldDefinition {Key = "key1"});

            check.Should().NotBeNull();
            check.Should().BeOfType(checkType);
        }

        [Test]
        public void IsValid_null_or_whitespace_should_pass()
        {
            var settings =
                new Dictionary<string, string>
                    {{"Hint", "Must be true or false"}, {"RegEx", "true|false"}} as IDictionary<string, string>;
            var ctx = new ContextFor<StringRegExCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            sut.ShouldPass(null, null);
            sut.ShouldPass(string.Empty, string.Empty);
            sut.ShouldPass("\t\r\n", "\t\r\n");
            sut.ShouldPass(" ", " ");
        }

        [Test]
        public void EmailChecks()
        {
            var settings =
                new Dictionary<string, string>
                    {{"Hint", "Not a valid e-mail"}, {"RegEx", @"^.+@.+\..+$"}} as IDictionary<string, string>;
            var ctx = new ContextFor<StringRegExCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            sut.ShouldPass("joe.doe@domain.com", "joe.doe@domain.com");
            sut.ShouldFailWith<ArgumentException>("@domain.com", null); // missing account prefix
            sut.ShouldFailWith<ArgumentException>("joe.doe_domain.com", null); // missing separator '@'
            sut.ShouldFailWith<ArgumentException>("joe.doe@", null); // missing domain
            sut.ShouldFailWith<ArgumentException>("joe.doe@domain_de", null); // missing domain separator '.'
        }

        [Test]
        public void IpAddressChecks()
        {
            var settings = new Dictionary<string, string>
                    {{"Hint", "Not a valid ip."}, {"RegEx", @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"}} as
                IDictionary<string, string>;
            var ctx = new ContextFor<StringRegExCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            sut.ShouldPass("1.1.1.1", "1.1.1.1");
            sut.ShouldPass("22.22.22.22", "22.22.22.22");
            sut.ShouldPass("333.333.333.333",
                "333.333.333.333"); // actually this is not a valid IP address. However, we check the RegEx here!

            sut.ShouldFailWith<ArgumentException>("1.1.11", 0); // missing separator
            sut.ShouldFailWith<ArgumentException>("1.22.33.4444", 0); // field with 4 digits
        }
    }
}