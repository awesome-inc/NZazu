using System;

namespace NZazu.Contracts.Checks
{
    public class RequiredCheck : IValueCheck
    {
        public void Validate(string value, IFormatProvider formatProvider = null)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ValidationException("This field is required.");
        }
    }
}