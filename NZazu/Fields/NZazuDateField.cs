using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields
{
    public class NZazuDateField : NZazuField<DateTime?>
    {
        // TODO: How to customize or inject the cultur/formatprovider?
        private IFormatProvider FormatProvider { get { return CultureInfo.InvariantCulture; } }

        private string _dateFormat;

        public NZazuDateField(string key) : base(key) { }

        public override string Type { get { return "date"; } }
        protected internal override DependencyProperty ContentProperty { get { return DatePicker.SelectedDateProperty; } }

        protected override Control GetValue()
        {
            var datePicker = new DatePicker { ToolTip = Description };
            String dateFormat;
            if (Settings != null && Settings.TryGetValue("Format", out dateFormat))
            {
                _dateFormat = dateFormat;
            }
            return datePicker;
        }

        protected override void SetStringValue(string value)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(_dateFormat))
                    Value = DateTime.Parse(value, FormatProvider);
                else
                    Value = DateTime.ParseExact(value, _dateFormat, FormatProvider);
            }
            catch (FormatException)
            {
                Value = null;
            }
        }

        protected override string GetStringValue()
        {
            if (!Value.HasValue) return String.Empty;

            var dateTime = Value.Value;
            if (String.IsNullOrWhiteSpace(_dateFormat))
                return dateTime.ToString(FormatProvider);
            return dateTime.ToString(_dateFormat, FormatProvider);
        }
    }
}