using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    public class StringLengthCheck : IValueCheck
    {
        private readonly ValueCheckResult _valueTooShort;
        private readonly ValueCheckResult _valueTooLong;

        public static string Name = "length";

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

        internal class StringLengthCheckSettings
        {
            public string Min { get; set; }
            public string Max { get; set; }
        }

        public static IValueCheck Create(IDictionary<string, string> settings)
        {
            var config = settings.ToDictionary(x => x.Key, x => (object) x.Value).ToObject<StringLengthCheckSettings>();

            Assert.Fail("implement me");

            return null;
            //if (config.Min)

            //if (!settings.ContainsKey("min")) throw new ArgumentException("Setting 'min' required. At least a minimum string length must be specified");
            //if (values == null || values.Count < 1) t
            //if (values.Count > 2) throw new ArgumentException("At most minimum and maximum string length can be specified");

            //if (values.Count == 1)
            //    return new StringLengthCheck(int.Parse(values[0]));
            //return new StringLengthCheck(int.Parse(values[0]), int.Parse(values[1]));
        }
    }
}