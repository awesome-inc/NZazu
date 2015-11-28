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
            if (check == null) throw new ArgumentNullException(nameof(check));
            _check = check;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var stringValue = Convert.ToString(value);
            var vr = _check.Validate(stringValue, cultureInfo);

            var error = vr.Error;
            var exception = error as Exception;
            if (exception != null)
                error = exception.Message;

            return new ValidationResult(vr.IsValid, error);
        }
    }
}