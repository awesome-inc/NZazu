using System;

namespace NZazu.Contracts.Checks
{
    public class StringLengthCheck : IValueCheck
    {
        public int MinimumLength { get; private set; }
        public int MaximumLength { get; private set; }

        // ReSharper disable IntroduceOptionalParameters.Global
        public StringLengthCheck(int min) : this(min, int.MaxValue) { }

        public StringLengthCheck(int min, int max)
        {
            if (min < 0) throw new ArgumentOutOfRangeException("min", "Minimum string length must not be negative.");
            if (max < min) throw new ArgumentOutOfRangeException(String.Format("min={0} must be less than or equal to max={1}.", min, max));
            MinimumLength = min;
            MaximumLength = max;
        }

        public void Validate(string value, IFormatProvider formatProvider = null)
        {
            if (String.IsNullOrWhiteSpace(value)) return;
            var length = value.Length;
            if (length < MinimumLength)
                throw new ValidationException(string.Format("The specified string is too short (at least {0} characters)", MinimumLength));
            if (length > MaximumLength)
                throw new ValidationException(string.Format("The specified string is too loong (at most {0} characters)", MaximumLength));
        }
    }
}