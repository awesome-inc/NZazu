using System;
using System.Globalization;

namespace NZazu.Contracts.Checks
{
    public class RangeCheck : IValueCheck
    {
        private readonly ValueCheckResult _outOfRange;
        private ValueCheckResult _notAValidNumber = new ValueCheckResult(false, new ArgumentException("The specified value is not a number"));

        public double Minimum { get; private set; }
        public double Maximum { get; private set; }

        public RangeCheck(double min, double max)
        {
            if (max.CompareTo(min) < 0) throw new ArgumentOutOfRangeException(
                $"min={min} must be less than or equal to max={max}.");
            Minimum = min;
            Maximum = max;

            _outOfRange = new ValueCheckResult(false, new ArgumentOutOfRangeException("value",
                $"The specified value must be between {Minimum} and {Maximum}"));
        }

        public ValueCheckResult Validate(string value, IFormatProvider formatProvider = null)
        {
            if (string.IsNullOrWhiteSpace(value)) return ValueCheckResult.Success;

            double result;
            var safeFormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
            if (double.TryParse(value, NumberStyles.Number, safeFormatProvider, out result))
            {
                if (result < Minimum || result > Maximum)
                    return _outOfRange;
                return ValueCheckResult.Success;
            }

            return _notAValidNumber;
        }
    }
}