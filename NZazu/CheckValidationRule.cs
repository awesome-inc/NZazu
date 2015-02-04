using System;
using System.Globalization;
using System.Windows.Controls;
using NZazu.Contracts.Checks;

namespace NZazu
{
    internal class CheckValidationRule : ValidationRule
    {
        private readonly IValueCheck _check;

        public CheckValidationRule(IValueCheck check)
        {
            if (check == null) throw new ArgumentNullException("check");
            _check = check;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var stringValue = value as string;
                _check.Validate(stringValue, cultureInfo);
                return new ValidationResult(true, null);
            }
            catch (ValidationException ex)
            {
                return new ValidationResult(false, ex.Message);
            }
        }
    }
}