using System;
using System.Globalization;

namespace NZazu.Contracts.Checks
{
    public class RangeCheck : IValueCheck
    {
        public int Minimum { get; private set; }
        public int Maximum { get; private set; }

        public RangeCheck(int min, int max)
        {
            if (max.CompareTo(min) < 0) throw new ArgumentOutOfRangeException(String.Format("min={0} must be less than or equal to max={1}.", min, max));
            Minimum = min;
            Maximum = max;
        }

        public void Validate(string value, CultureInfo cultureInfo = null)
        {
            try
            {
                var safeCultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
                var intVal = int.Parse(value, safeCultureInfo);
                if (intVal < Minimum || intVal > Maximum)
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