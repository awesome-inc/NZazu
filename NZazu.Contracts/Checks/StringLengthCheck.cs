using System;

namespace NZazu.Contracts.Checks
{
    public class StringLengthCheck : IValueCheck
    {
        private readonly ValueCheckResult _valueTooShort;
        private readonly ValueCheckResult _valueTooLong;

        public int MinimumLength { get; private set; }
        public int MaximumLength { get; private set; }

        // ReSharper disable IntroduceOptionalParameters.Global
        public StringLengthCheck(int min) : this(min, int.MaxValue) { }

        public StringLengthCheck(int min, int max)
        {
            if (min < 0) throw new ArgumentOutOfRangeException("min", "Minimum string length must not be negative.");
            if (max < min) throw new ArgumentOutOfRangeException(string.Format("min={0} must be less than or equal to max={1}.", min, max));
            MinimumLength = min;
            MaximumLength = max;

            _valueTooShort = new ValueCheckResult(false, new ArgumentException(string.Format("The specified string is too short (at least {0} characters)", MinimumLength)));
            _valueTooLong = new ValueCheckResult(false, new ArgumentException(string.Format("The specified string is too loong (at most {0} characters)", MaximumLength)));
        }

        public ValueCheckResult Validate(string value, IFormatProvider formatProvider = null)
        {
            if (string.IsNullOrWhiteSpace(value)) return ValueCheckResult.Success;
            
            var length = value.Length;
            if (length < MinimumLength)
                return _valueTooShort;
            if (length > MaximumLength)
                return _valueTooLong;

            return ValueCheckResult.Success;
        }
    }
}