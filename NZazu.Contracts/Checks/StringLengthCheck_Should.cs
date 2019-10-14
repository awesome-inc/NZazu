using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(StringLengthCheck))]
    // ReSharper disable InconsistentNaming
    internal class StringLengthCheck_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var settings = new Dictionary<string, string> {{"Min", "2"}, {"Max", "6"}} as IDictionary<string, string>;
            var ctx = new ContextFor<StringLengthCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            sut.Should().NotBeNull();
            sut.GetType().GetCustomAttribute<DisplayNameAttribute>().DisplayName.Should().Be("length");
        }

        [Test]
        public void Registered_At_CheckFactory()
        {
            var settings = new Dictionary<string, string> {{"Min", "2"}, {"Max", "6"}} as IDictionary<string, string>;
            var checkType = typeof(StringLengthCheck);

            var sut = new CheckFactory();

            var checkDefinition = new CheckDefinition {Type = "length", Settings = settings};
            var check = sut.CreateCheck(checkDefinition, new FieldDefinition {Key = "key1"});

            check.Should().NotBeNull();
            check.Should().BeOfType(checkType);
        }

        [Test]
        public void Ctor_MaxLessOrEqualMin_Should_Throw()
        {
            var settings = new Dictionary<string, string> {{"Min", "6"}, {"Max", "2"}} as IDictionary<string, string>;

            // ReSharper disable once ObjectCreationAsStatement
            new Action(() => new StringLengthCheck(settings, null, null, 0, null)).Invoking(a => a()).Should()
                .Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Ctor_NegativeLength_Should_Throw()
        {
            var settings = new Dictionary<string, string> {{"Min", "-2"}} as IDictionary<string, string>;

            // ReSharper disable once ObjectCreationAsStatement
            new Action(() => new StringLengthCheck(settings, null, null, 0, null)).Invoking(a => a()).Should()
                .Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void IsValid_BelowMin_fails()
        {
            var settings = new Dictionary<string, string> {{"Min", "2"}, {"Max", "42"}} as IDictionary<string, string>;
            var ctx = new ContextFor<StringLengthCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            var candidate = new string('A', 1);
            sut.ShouldFailWith<ArgumentException>(candidate, candidate);
        }

        [Test]
        public void IsValid_AboveMax_fails()
        {
            var settings = new Dictionary<string, string> {{"Min", "2"}, {"Max", "42"}} as IDictionary<string, string>;
            var ctx = new ContextFor<StringLengthCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            var candidate = new string('A', 100);
            sut.ShouldFailWith<ArgumentException>(candidate, candidate);
        }

        [Test]
        public void IsValid_InsideMinMax_passes()
        {
            var settings = new Dictionary<string, string> {{"Min", "2"}, {"Max", "42"}} as IDictionary<string, string>;
            var ctx = new ContextFor<StringLengthCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            Enumerable.Range(2, 40)
                .Select(val => new string('A', val))
                .ToList().ForEach(x => sut.ShouldPass(x, x));
        }

        [Test]
        public void IsValid_NullOrWhitespace_passes()
        {
            var ctx = new ContextFor<StringLengthCheck>();
            var sut = ctx.BuildSut();

            sut.ShouldPass(null, null);
            sut.ShouldPass(string.Empty, string.Empty);
            sut.ShouldPass("\t\r\n", "\t\r\n");
            sut.ShouldPass(" ", " ");
        }

        [Test]
        public void Ctor_with_min_only_should_set_max_to_int_maxvalue()
        {
            var settings = new Dictionary<string, string> {{"Min", "4"}} as IDictionary<string, string>;
            var ctx = new ContextFor<StringLengthCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            sut.Settings.MinInt.Should().Be(4);
            sut.Settings.MaxInt.Should().Be(int.MaxValue);
        }
    }
}