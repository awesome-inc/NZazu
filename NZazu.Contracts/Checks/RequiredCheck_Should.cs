using System;
using System.Collections.Generic;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(RequiredCheck))]
    // ReSharper disable InconsistentNaming
    internal class RequiredCheck_Should
    {
        [Test]
        public void Be_Creatable()
        {
            Assert.Fail("should be implemented");
        }

        [Test]
        public void Registered_At_CheckFactory()
        {
            var type = RequiredCheck.Name;
            var checkType = typeof(RequiredCheck);
            var settings = new Dictionary<string, string>();

            var sut = new CheckFactory();

            var checkDefinition = new CheckDefinition { Type = type, Settings = settings };

            var check = sut.CreateCheck(checkDefinition);

            check.Should().NotBeNull();
            check.Should().BeOfType(checkType);
        }

        [Test]
        public void Throw_ValidationException_if_value_null_or_whitespace()
        {
            var check = new RequiredCheck();

            check.ShouldFailWith<ArgumentException>(null, null);
            check.ShouldFailWith<ArgumentException>(string.Empty, string.Empty);
            check.ShouldFailWith<ArgumentException>("\t\r\n", "\t\r\n");
            check.ShouldFailWith<ArgumentException>(" ", " ");

            check.Validate("a", "a").IsValid.Should().BeTrue();
        }
    }
}