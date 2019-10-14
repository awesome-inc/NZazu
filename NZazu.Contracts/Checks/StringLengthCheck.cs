using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NZazu.Contracts.Checks
{
    [DisplayName("length")]
    public class StringLengthCheck : IValueCheck
    {
        private readonly ValueCheckResult _valueTooLong;

        private readonly ValueCheckResult _valueTooShort;

        internal readonly StringLengthCheckSettings Settings;

        internal class StringLengthCheckSettings
        {
            public string Min { private get; set; }
            public string Max { private get; set; }

            public int MinInt => Convert.ToInt32(Min ?? "0");
            public int MaxInt => Convert.ToInt32(Max ?? int.MaxValue.ToString());
        }

        // ReSharper disable UnusedParameter.Local
        public StringLengthCheck(
            IDictionary<string, string> settings, Func<FormData> formData,
            INZazuTableDataSerializer tableSerializer, int rowIdx, FieldDefinition field)
        {
            Settings = settings.ToDictionary(x => x.Key, x => (object) x.Value).ToObject<StringLengthCheckSettings>();

            if (Settings.MinInt < 0)
                throw new ArgumentOutOfRangeException(nameof(Settings.Min),
                    $"Minimum string length '{Settings.MinInt}' must not be negative.");
            if (Settings.MaxInt < Settings.MinInt)
                throw new ArgumentOutOfRangeException(
                    $"min={Settings.MinInt} must be less than or equal to max={Settings.MaxInt}.");

            _valueTooShort = new ValueCheckResult(false, new ArgumentException(
                $"The specified string is too short (at least {Settings.MinInt} characters)"));
            _valueTooLong = new ValueCheckResult(false, new ArgumentException(
                $"The specified string is too long (at most {Settings.MaxInt} characters)"));
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            if (string.IsNullOrWhiteSpace(value)) return ValueCheckResult.Success;

            var length = value.Length;
            if (length < Settings.MinInt) return _valueTooShort;
            if (length > Settings.MaxInt) return _valueTooLong;

            return ValueCheckResult.Success;
        }
    }
}