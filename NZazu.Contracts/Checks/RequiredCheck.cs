using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NZazu.Contracts.Checks
{
    [DisplayName("required")]
    public class RequiredCheck : IValueCheck
    {
        private static readonly ValueCheckResult FieldMissing = new ValueCheckResult(new ArgumentException("This field is required"));

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            return string.IsNullOrWhiteSpace(value) ? FieldMissing : ValueCheckResult.Success;
        }

        public static IValueCheck Create(IDictionary<string, string> settings)
        {
            return new RequiredCheck();
        }
    }
}