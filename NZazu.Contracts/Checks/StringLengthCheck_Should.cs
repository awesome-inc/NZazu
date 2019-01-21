using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
            Assert.Fail("should be implemented");
        }

        [Test]
        public void Registered_At_CheckFactory()
        {
            var type = StringLengthCheck.Name;
            var checkType = typeof(StringLengthCheck);
            var settings = new Dictionary<string, string>() { { "Min", "-42" }, { "Max", "42" } };

            var sut = new CheckFactory();

            var checkDefinition = new CheckDefinition { Type = type, Settings = settings };

            var check = sut.CreateCheck(checkDefinition);

            check.Should().NotBeNull();
            check.Should().BeOfType(checkType);
        }

        [Test]
        public void Ctor_MaxLEQMin_Should_Throw()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Action(() => new StringLengthCheck(1, 0)).Invoking(a => a()).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Ctor_NegativeLength_Should_Throw()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Action(() => new StringLengthCheck(-1, -1)).Invoking(a => a()).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void IsValid_BelowMin_fails()
        {
            var _check = new StringLengthCheck(4, 6);
            var candidate = new string('A', _check.MinimumLength - 1);
            _check.ShouldFailWith<ArgumentException>(candidate, candidate);
        }

        [Test]
        public void IsValid_AboveMax_fails()
        {
            var _check = new StringLengthCheck(4, 6);
            var candidate = new string('A', _check.MaximumLength + 1);
            _check.ShouldFailWith<ArgumentException>(candidate, candidate);
        }

        [Test]
        public void IsValid_InsideMinMax_passes()
        {
            var _check = new StringLengthCheck(4, 6);
            Enumerable.Range(_check.MinimumLength, _check.MaximumLength - _check.MinimumLength)
                .Select(val => new string('A', val))
                .ToList().ForEach(x => _check.ShouldPass(x, x));
        }

        [Test]
        public void IsValid_NullOrWhitespace_passes()
        {
            var check = new StringLengthCheck(3, 4);
            check.ShouldPass(null, null);
            check.ShouldPass(string.Empty, string.Empty);
            check.ShouldPass("\t\r\n", "\t\r\n");
            check.ShouldPass(" ", " ");
        }

        [Test]
        public void Ctor_with_min_only_should_set_max_to_int_maxvalue()
        {
            const int min = 4;
            var sut = new StringLengthCheck(min);
            sut.MinimumLength.Should().Be(min);
            sut.MaximumLength.Should().Be(int.MaxValue);
        }
    }
}