using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;

namespace NZazu.Fields
{
    internal class NZazuDoubleField : NZazuField<double?>
    {
        public NZazuDoubleField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
        }

        public override DependencyProperty ContentProperty => TextBox.TextProperty;

        protected override Control CreateValueControl()
        {
            var result = new TextBox {ToolTip = Definition.Description};
            return result;
        }

        public override void SetValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                double.TryParse(value, NumberStyles.Number, FormatProvider, out var result))
                Value = result;
            else
                Value = null;
        }

        public override string GetValue()
        {
            return Value.HasValue
                ? Value.Value.ToString(FormatProvider)
                : string.Empty;
        }

        protected internal override Binding DecorateBinding(Binding binding)
        {
            var format = Definition.Settings.ContainsKey("Format") ? Definition.Settings["Format"] : "G";
            var decorated = base.DecorateBinding(binding);
            decorated.Converter = new DoubleToStringConverter(format);
            return decorated;
        }

        #region converter

        internal class DoubleToStringConverter : IValueConverter
        {
            private readonly CultureInfo _culture;
            private readonly string _format;
            private string _attachDot = string.Empty;
            private string _lastValue;

            public DoubleToStringConverter(string format = "G", CultureInfo culture = null)
            {
                _format = format;
                _culture = culture;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (_lastValue != null) return _lastValue; // to make sure the textbox keeps the "invalid" text

                var cultureSafe = _culture ?? culture ?? Thread.CurrentThread.CurrentUICulture;
                var result = (double?) value;
                return (result.HasValue ? result.Value.ToString(_format, cultureSafe) : string.Empty) + _attachDot;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                _lastValue = null; // we assume it will work

                var cultureSafe = _culture ?? culture ?? Thread.CurrentThread.CurrentUICulture;
                var separator = cultureSafe.NumberFormat.CurrencyDecimalSeparator;

                double? result = null;
                var str = (string) value ?? string.Empty;
                _attachDot = str.EndsWith(separator) ? separator : string.Empty;

                double d;
                if (double.TryParse(str, NumberStyles.Float, cultureSafe, out d))
                    result = d;
                else
                    _lastValue = str;

                return result;
            }
        }

        #endregion
    }
}