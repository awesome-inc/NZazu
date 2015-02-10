using System;
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
    }
}