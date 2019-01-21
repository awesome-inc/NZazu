using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using NEdifis;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(RequiredCheck))]
    // ReSharper disable InconsistentNaming
    internal class RequiredCheck_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var ctx = new ContextFor<RequiredCheck>();
            var sut = ctx.BuildSut();

            sut.Should().NotBeNull();

            sut.GetType().GetCustomAttribute<DisplayNameAttribute>().DisplayName.Should().Be("required");
        }

        [Test]
        public void Registered_At_CheckFactory()
        {
            var checkType = typeof(RequiredCheck);
            var settings = new Dictionary<string, string>();

            var sut = new CheckFactory();

            var checkDefinition = new CheckDefinition { Type = "required", Settings = settings };

            var check = sut.CreateCheck(checkDefinition);

            check.Should().NotBeNull();
            check.Should().BeOfType(checkType);
        }

        [Test]
        public void Throw_ValidationException_if_value_null_or_whitespace()
        {
            var ctx = new ContextFor<RequiredCheck>();
            var sut = ctx.BuildSut();

            sut.ShouldFailWith<ArgumentException>(null, null);
            sut.ShouldFailWith<ArgumentException>(string.Empty, string.Empty);
            sut.ShouldFailWith<ArgumentException>("\t\r\n", "\t\r\n");
            sut.ShouldFailWith<ArgumentException>(" ", " ");

            sut.Validate("a", "a").IsValid.Should().BeTrue();
        }
    }
}