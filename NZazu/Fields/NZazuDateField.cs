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

        public string DateFormat { get; protected set; }

        public NZazuDateField(string key) : base(key) { }

        public override string Type { get { return "date"; } }
        public override DependencyProperty ContentProperty { get { return DatePicker.SelectedDateProperty; } }

        protected override Control GetValue()
        {
            DateFormat = GetDateFormat();
            return new DatePicker {ToolTip = Description};
        }

        protected override void SetStringValue(string value)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(DateFormat))
                    Value = DateTime.Parse(value, FormatProvider);
                else
                    Value = DateTime.ParseExact(value, DateFormat, FormatProvider);
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
            if (String.IsNullOrWhiteSpace(DateFormat))
                return dateTime.ToString(FormatProvider);
            return dateTime.ToString(DateFormat, FormatProvider);
        }

        protected string GetDateFormat()
        {
            String dateFormat = null;
            if (Settings != null) Settings.TryGetValue("Format", out dateFormat);
            return dateFormat;
        }
    }
}