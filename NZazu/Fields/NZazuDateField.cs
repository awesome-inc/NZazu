using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields
{
    public class NZazuDateField : NZazuField<DateTime?>
    {
        public string DateFormat { get; protected internal set; }

        public NZazuDateField(string key) : base(key) { }

        public override string Type { get { return "date"; } }
        public override DependencyProperty ContentProperty { get { return DatePicker.SelectedDateProperty; } }

        protected override Control GetValue()
        {
            DateFormat = GetSetting("Format");
            return new DatePicker {ToolTip = Description};
        }

        protected override void SetStringValue(string value)
        {
            var parsed = false;
            var result = new DateTime();

            if (!String.IsNullOrWhiteSpace(value))
            {
                const DateTimeStyles dateTimeStyles = DateTimeStyles.AssumeLocal;
                parsed = String.IsNullOrWhiteSpace(DateFormat) 
                    ? DateTime.TryParse(value, FormatProvider, dateTimeStyles, out result) 
                    : DateTime.TryParseExact(value, DateFormat, FormatProvider, dateTimeStyles, out result);
            }
            if (parsed)
                Value = result;
            else
                Value = null;
        }

        protected override string GetStringValue()
        {
            if (!Value.HasValue) return String.Empty;

            var dateTime = Value.Value;
            if (String.IsNullOrWhiteSpace(DateFormat))
                return dateTime.ToString(FormatProvider);
            return dateTime.ToString(DateFormat, FormatProvider);
        }
    }
}