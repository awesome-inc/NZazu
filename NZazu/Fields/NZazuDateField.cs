using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields
{
    class NZazuDateField : NZazuField<DateTime?>
    {
        public NZazuDateField(string key) : base(key) { }

        public override string Type { get { return "date"; } }
        protected internal override DependencyProperty ContentProperty { get { return DatePicker.SelectedDateProperty; } }

        protected override Control GetValue()
        {
            var result = new DatePicker {ToolTip = Description};
            return result;
        }

        protected override void SetStringValue(string value)
        {
            Console.WriteLine(value);
            try
            {
                var date = DateTime.Parse(value, CultureInfo.InvariantCulture);
                Value = date;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Value = null;
            }
        }
        // todo culture
        protected override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;
        }
    }
}