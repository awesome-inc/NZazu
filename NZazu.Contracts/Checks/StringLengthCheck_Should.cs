using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class StringLengthCheck_Should
    {
        [Test]
        public void Ctor_MaxLEQMin_ShouldThrow()
        {
            new Action(() => new StringLengthCheck(1, 0)).Invoking(a => a()).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Ctor_NegativeLength_ShouldThrow()
        {
            new Action(() => new StringLengthCheck(-1, -1)).Invoking(a => a()).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void IsValid_BelowMin_Throws_ValidationException()
        {
            var _check = new StringLengthCheck(4, 6);
            var candidate = new String('A', _check.MinimumLength - 1);
            Assert.Throws<ValidationException>(() => _check.Validate(candidate));
        }

        [Test]
        public void IsValid_AboveMax_Throws_ValidationException()
        {
            var _check = new StringLengthCheck(4, 6);
            var candidate = new String('A', _check.MaximumLength + 1);
            Assert.Throws<ValidationException>(() => _check.Validate(candidate));
        }

        [Test]
        public void IsValid_InsideMinMax_Should_Pass()
        {
            var _check = new StringLengthCheck(4, 6);
            Enumerable.Range(_check.MinimumLength, _check.MaximumLength - _check.MinimumLength)
                .Select(val => new String('A', val))
                .ToList().ForEach(candidate => _check.Validate(candidate));
        }

        [Test]
        public void IsValid_HandleNullString_AsLengthZero()
        {
            var check = new StringLengthCheck(0, 1);
            check.Validate(null);
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