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
            Enumerable.Range(_check.Minimum, _check.Maximum - _check.Minimum)
                    .Select(val => val.ToString(cultureInfo))
                    .ToList().ForEach(val => _check.Validate(val, cultureInfo));
        }

        [Test]
        public void Validate_Value_NullOrWhiteSpace_Throws_ValidationException()
        {
            Assert.Throws<ValidationException>(() => _check.Validate(null));
            Assert.Throws<ValidationException>(() => _check.Validate(String.Empty));
            Assert.Throws<ValidationException>(() => _check.Validate("\t\r\n"));
            Assert.Throws<ValidationException>(() => _check.Validate(" "));
        }

        [Test]
        public void Validate_Should_Rethrow_Overflow_as_ValidationException()
        {
            const long longValue = int.MaxValue + 1L;
            var value = longValue.ToString(CultureInfo.InvariantCulture);
            Assert.Throws<ValidationException>(() => _check.Validate(value));
        }
    }
}