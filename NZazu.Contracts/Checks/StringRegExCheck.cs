using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace NZazu.Contracts.Checks
{
    [DisplayName("regex")]
    public class StringRegExCheck : IValueCheck
    {
        private readonly ValueCheckResult _noMatch;

        internal readonly StringRegExCheckSettings Settings;

        internal class StringRegExCheckSettings
        {
            public string Hint { get; set; }
            public string RegEx { get; set; }
        }

        // ReSharper disable UnusedParameter.Local
        public StringRegExCheck(
            IDictionary<string, string> settings, Func<FormData> formData,
            INZazuTableDataSerializer tableSerializer, int rowIdx,
            FieldDefinition fieldDefinition)
        {
            Settings = settings.ToDictionary(x => x.Key, x => (object) x.Value).ToObject<StringRegExCheckSettings>();

            if (string.IsNullOrWhiteSpace(Settings.Hint)) throw new ArgumentNullException(nameof(Settings.Hint));
            if (string.IsNullOrWhiteSpace(Settings.RegEx)) throw new ArgumentNullException(nameof(Settings.RegEx));

            _noMatch = new ValueCheckResult(false, new ArgumentException(Settings.Hint));
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            if (string.IsNullOrWhiteSpace(value)) return ValueCheckResult.Success;

            var regex = new Regex(Settings.RegEx);

            var match = regex.IsMatch(value);
            return match ? ValueCheckResult.Success : _noMatch;
        }
    }
}