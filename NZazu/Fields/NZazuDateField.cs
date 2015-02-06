using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
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
            var result = new DatePicker { ToolTip = Description };
            result.ApplyTemplate();
            var textBox = (TextBox)result.GetType()
                .GetProperty("TextBox", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result);
            var binding = textBox.GetBindingExpression(TextBox.TextProperty).ParentBinding;
            binding.StringFormat = Settings["Format"];
            return result;
        }

        protected override void SetStringValue(string value)
        {
            try
            {
                var date = DateTime.Parse(value, CultureInfo.InvariantCulture);
                Value = date;
            }
            catch (FormatException)
            {
                Value = null;
            }
        }
        protected override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;
        }
    }
}