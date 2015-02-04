using System;
using System.Globalization;

namespace NZazu.Contracts.Checks
{
    public class RequiredCheck : IValueCheck
    {
        public void Validate(string value, CultureInfo cultureInfo = null)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ValidationException("This field is required.");
        }
    }
}