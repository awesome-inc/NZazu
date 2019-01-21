using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NZazu.Contracts.Checks
{
    public class RangeCheck : IValueCheck
    {
        public static string Name = "range";

        private readonly ValueCheckResult _outOfRange;
        private readonly ValueCheckResult _notAValidNumber = new ValueCheckResult(false, new ArgumentException("The specified value is not a number"));

        public double Minimum { get; }
        public double Maximum { get; }

        public RangeCheck(double min, double max)
        {
            if (max.CompareTo(min) < 0) throw new ArgumentOutOfRangeException(
                $"min={min} must be less than or equal to max={max}.");
            Minimum = min;
            Maximum = max;
            _outOfRange = new ValueCheckResult(false, new ArgumentOutOfRangeException(
                $"The specified value must be between {Minimum} and {Maximum}"));
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
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

        public static IValueCheck Create(IDictionary<string, string> settings)
        {
            Assert.Fail("implement me");
            return null;

            //if (values == null || values.Count != 2) throw new ArgumentException("Must sepcify minimum and maximum");
            //return new RangeCheck(double.Parse(values[0], CultureInfo.InvariantCulture), double.Parse(values[1], CultureInfo.InvariantCulture));

        }
    }
}