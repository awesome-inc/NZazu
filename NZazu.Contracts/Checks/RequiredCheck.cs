using System;

namespace NZazu.Contracts.Checks
{
    public class RequiredCheck : IValueCheck
    {
        private static readonly ValueCheckResult FieldMissing = new ValueCheckResult(new ArgumentException("This field is required."));

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            return string.IsNullOrWhiteSpace(value) ? FieldMissing : ValueCheckResult.Success;
        }
    }
}