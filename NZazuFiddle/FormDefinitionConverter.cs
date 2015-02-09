using System;
using System.Globalization;
using System.Windows.Data;
using Newtonsoft.Json;
using NZazu.Contracts;

namespace NZazuFiddle
{
    [ValueConversion(typeof(FormDefinition), typeof(string))]
    public class FormDefinitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var formDefinition = (FormDefinition)value;
            return JsonConvert.SerializeObject(formDefinition, Formatting.Indented);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var json = (string)value;
                return JsonConvert.DeserializeObject<FormDefinition>(json);

            }
            catch (Exception) { return value; }
        }
    }
}