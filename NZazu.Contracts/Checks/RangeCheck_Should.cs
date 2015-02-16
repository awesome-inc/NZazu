using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class RangeCheck_Should
    {
        private RangeCheck _check;

        [SetUp]
        public void SetUp()
        {
            _check = new RangeCheck(0, 10);
        }

        [Test]
        public void Ctor_MaxLEQMin_ShouldThrow()
        {
            new Action(() => new RangeCheck(1, 0)).Invoking(a => a()).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Validate_BelowMin_Throws_ValidationException()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var value = (_check.Minimum - 1).ToString(cultureInfo);
            Assert.Throws<ValidationException>(() => _check.Validate(value, cultureInfo));
        }

        [Test]
        public void Validate_AboveMax_Throws_ValidationException()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var value = (_check.Maximum + 1).ToString(cultureInfo);
            Assert.Throws<ValidationException>(() => _check.Validate(value, cultureInfo));
        }

        [Test]
        public void Validate_InsideMinMax_Should_Pass()
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            Enumerable.Range((int) _check.Minimum, (int)(_check.Maximum - _check.Minimum))
                    .Select(val => val.ToString(cultureInfo))
                    .ToList().ForEach(val => _check.Validate(val, cultureInfo));
        }

        [Test]
        public void Validate_Value_NullOrWhiteSpace_Passes()
        {
            _check.Validate(null);
            _check.Validate(String.Empty);
            _check.Validate("\t\r\n");
            _check.Validate(" ");
        }

        [Test]
        public void Validate_Should_Rethrow_Overflow_as_ValidationException()
        {
            const long longValue = int.MaxValue + 1L;
            var value = longValue.ToString(CultureInfo.InvariantCulture);
            Assert.Throws<ValidationException>(() => _check.Validate(value));
        }

        [Test]
        public void Validate_Rethrows_OverflowException_as_ValidationException()
        {
            var culture = CultureInfo.InvariantCulture;
            var value = double.MaxValue.ToString(culture);
            _check.Invoking(x => x.Validate(value, culture))
                .ShouldThrow<ValidationException>()
                .WithInnerException<OverflowException>();
        }
    }
}