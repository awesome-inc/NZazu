using System;

namespace NZazu.Contracts.Checks
{
    public class StringLengthCheck : IValueCheck
    {
        private readonly ValueCheckResult _valueTooShort;
        private readonly ValueCheckResult _valueTooLong;

        public int MinimumLength { get; }
        public int MaximumLength { get; }

        // ReSharper disable IntroduceOptionalParameters.Global
        public StringLengthCheck(int min) : this(min, int.MaxValue) { }

        public StringLengthCheck(int min, int max)
        {
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min), "Minimum string length must not be negative.");
            if (max < min) throw new ArgumentOutOfRangeException($"min={min} must be less than or equal to max={max}.");
            MinimumLength = min;
            MaximumLength = max;

            _valueTooShort = new ValueCheckResult(false, new ArgumentException(
                $"The specified string is too short (at least {MinimumLength} characters)"));
            _valueTooLong = new ValueCheckResult(false, new ArgumentException(
                $"The specified string is too long (at most {MaximumLength} characters)"));
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            //if (string.IsNullOrWhiteSpace(value)) return ValueCheckResult.Success;
            
            var length = value.Length;
            if (length < MinimumLength)
                return _valueTooShort;
            if (length > MaximumLength)
                return _valueTooLong;

            return ValueCheckResult.Success;
        }
    }
}