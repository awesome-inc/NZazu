using System;

namespace NZazu.Contracts.Checks
{
    public class RequiredCheck : IValueCheck
    {
        public static readonly ValueCheckResult FieldMissing = new ValueCheckResult(false, new ArgumentException("This field is required."));

        public ValueCheckResult Validate(string value, IFormatProvider formatProvider = null)
        {
            return string.IsNullOrWhiteSpace(value) ? FieldMissing : ValueCheckResult.Success;
        }
    }
}