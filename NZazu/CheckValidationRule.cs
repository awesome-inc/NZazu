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
            _check = check ?? throw new ArgumentNullException(nameof(check));
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var stringValue = Convert.ToString(value);
            var vr = _check.Validate(stringValue, stringValue, cultureInfo ?? CultureInfo.CurrentCulture);

            var error = vr.Exception;

            return new ValidationResult(vr.IsValid, error?.Message ?? string.Empty);
        }
    }
}