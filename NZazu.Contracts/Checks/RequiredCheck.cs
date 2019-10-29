using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NZazu.Contracts.Checks
{
    [DisplayName("required")]
    public class RequiredCheck : IValueCheck
    {
        private readonly ValueCheckResult _fieldMissing;

        public RequiredCheck(
            IDictionary<string, string> settings,
            Func<FormData> formData,
            INZazuTableDataSerializer tableSerializer,
            int rowIdx,
            FieldDefinition field)
        {
            _fieldMissing =
                new ValueCheckResult(new ArgumentException($"{field.Prompt ?? field.Key}: The field is required"));
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            return string.IsNullOrWhiteSpace(value) ? _fieldMissing : ValueCheckResult.Success;
        }
    }
}