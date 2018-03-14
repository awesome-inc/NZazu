using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(RangeCheck))]
    // ReSharper disable InconsistentNaming
    internal class RangeCheck_Should
    {
        private RangeCheck _check;

        [SetUp]
        public void SetUp()
        {
            _check = new RangeCheck(0, 10);
        }

        [Test]
        public void Ctor_MaxLEQMin_Should_Throw()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Action(() => new RangeCheck(1, 0)).Invoking(a => a()).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Validate_BelowMin_fails()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var value = (_check.Minimum - 1).ToString(cultureInfo);
            var vr = _check.Validate(value, cultureInfo);
            vr.IsValid.Should().BeFalse();
            vr.Error.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Validate_AboveMax_fails()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var value = (_check.Maximum + 1).ToString(cultureInfo);
            var vr = _check.Validate(value, cultureInfo);
            vr.IsValid.Should().BeFalse();
            vr.Error.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Validate_InsideMinMax_Should_Pass()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            Enumerable.Range((int) _check.Minimum, (int)(_check.Maximum - _check.Minimum))
                    .Select(val => val.ToString(cultureInfo))
                    .ToList().ForEach(val => _check.Validate(val, cultureInfo).IsValid.Should().BeTrue());
        }

        [Test]
        public void Validate_Value_NullOrWhiteSpace_Passes()
        {
            _check.ShouldPass(null);
            _check.ShouldPass(string.Empty);
            _check.ShouldPass("\t\r\n");
            _check.ShouldPass(" ");
        }

        [Test]
        public void Validate_NaN_fails()
        {
            var vr = _check.Validate("not a number");
            vr.IsValid.Should().BeFalse();
            vr.Error.Should().BeOfType<ArgumentException>();
        }
    }
}