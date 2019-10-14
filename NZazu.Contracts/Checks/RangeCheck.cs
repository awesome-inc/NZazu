using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace NZazu.Contracts.Checks
{
    [DisplayName("range")]
    public class RangeCheck : IValueCheck
    {
        private readonly ValueCheckResult _notAValidNumber =
            new ValueCheckResult(false, new ArgumentException("The specified value is not a number"));

        private readonly ValueCheckResult _outOfRange;

        internal readonly RangeCheckSettings Settings;

        internal class RangeCheckSettings
        {
            public string Min { private get; set; }
            public string Max { private get; set; }

            public int MinInt => Convert.ToInt32(Min ?? "0");
            public int MaxInt => Convert.ToInt32(Max ?? int.MaxValue.ToString());
        }

        // ReSharper disable UnusedParameter.Local
        public RangeCheck(
            IDictionary<string, string> settings, Func<FormData> formData,
            INZazuTableDataSerializer tableSerializer, int rowIdx,
            FieldDefinition fieldDefinition)
        {
            Settings = settings.ToDictionary(x => x.Key, x => (object) x.Value).ToObject<RangeCheckSettings>();

            if (Settings.MinInt < 0)
                throw new ArgumentOutOfRangeException(nameof(Settings.Min),
                    $"The specified value must be between {Settings.MinInt} and {Settings.MaxInt}");
            if (Settings.MaxInt < Settings.MinInt)
                throw new ArgumentOutOfRangeException(
                    $"min={Settings.MinInt} must be less than or equal to max={Settings.MaxInt}.");

            _outOfRange = new ValueCheckResult(false, new ArgumentOutOfRangeException(
                $"The specified value must be between {Settings.MinInt} and {Settings.MaxInt}"));
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            if (string.IsNullOrWhiteSpace(value)) return ValueCheckResult.Success;

            var safeFormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
            if (!double.TryParse(value, NumberStyles.Number, safeFormatProvider, out var result))
                return _notAValidNumber;

            if (result < Settings.MinInt || result > Settings.MaxInt)
                return _outOfRange;

            return ValueCheckResult.Success;
        }
    }
}