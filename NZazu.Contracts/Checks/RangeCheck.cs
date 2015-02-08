using System;
using System.Globalization;

namespace NZazu.Contracts.Checks
{
    public class RangeCheck : IValueCheck
    {
        public double Minimum { get; private set; }
        public double Maximum { get; private set; }

        public RangeCheck(double min, double max)
        {
            if (max.CompareTo(min) < 0) throw new ArgumentOutOfRangeException(String.Format("min={0} must be less than or equal to max={1}.", min, max));
            Minimum = min;
            Maximum = max;
        }

        public void Validate(string value, IFormatProvider formatProvider = null)
        {
            try
            {
                var safeFormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
                var val = double.Parse(value, safeFormatProvider);
                if (val < Minimum || val > Maximum)
                    throw new ValidationException(string.Format("The specified value must be between {0} and {1}",
                        Minimum, Maximum));
            }
            catch (FormatException fEx)
            {
                throw new ValidationException("The specified value is not a number", fEx);
            }
            catch (OverflowException oEx)
            {
                throw new ValidationException("The specified value is not a valid number", oEx);
            }
            catch (ArgumentNullException aEx)
            {
                throw new ValidationException("The specified value is not a valid number", aEx);
            }
        }
    }
}