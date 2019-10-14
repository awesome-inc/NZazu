using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuDateField : NZazuField<DateTime?>
    {
        public NZazuDateField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
        }

        public string DateFormat { get; protected internal set; }

        public override DependencyProperty ContentProperty => DatePicker.SelectedDateProperty;

        protected override Control CreateValueControl()
        {
            DateFormat = Definition.Settings.Get("Format");
            return new DatePicker {ToolTip = Definition.Description};
        }

        public override void SetValue(string value)
        {
            var parsed = false;
            var result = new DateTime();

            if (!string.IsNullOrWhiteSpace(value))
            {
                const DateTimeStyles dateTimeStyles = DateTimeStyles.AssumeLocal;
                parsed = string.IsNullOrWhiteSpace(DateFormat)
                    ? DateTime.TryParse(value, FormatProvider, dateTimeStyles, out result)
                    : DateTime.TryParseExact(value, DateFormat, FormatProvider, dateTimeStyles, out result);
            }

            if (parsed)
                Value = result;
            else
                Value = null;
        }

        public override string GetValue()
        {
            if (!Value.HasValue) return string.Empty;

            var dateTime = Value.Value;
            if (string.IsNullOrWhiteSpace(DateFormat))
                return dateTime.ToString(FormatProvider);
            return dateTime.ToString(DateFormat, FormatProvider);
        }
    }
}