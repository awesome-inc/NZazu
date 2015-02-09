using System;
using System.Globalization;
using System.Windows.Data;
using Newtonsoft.Json;
using NZazu.Contracts;

namespace NZazuFiddle
{
    [ValueConversion(typeof (FormData), typeof (string))]
    public class FormDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var formData = (FormData) value;
            return JsonConvert.SerializeObject(formData, Formatting.Indented);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var json = (string)value;
                return JsonConvert.DeserializeObject<FormData>(json);

            }
            catch (Exception) { return value; }
        }
    }
}