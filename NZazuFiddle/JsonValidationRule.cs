using System.Globalization;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace NZazuFiddle
{
    public class JsonValidationRule<T> : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var json = (string)value;
                JsonConvert.DeserializeObject<T>(json);
                return new ValidationResult(true, null);
            }
            catch (JsonSerializationException ex)
            {
                return new ValidationResult(false, ex.Message);
            }
        }
    }
}