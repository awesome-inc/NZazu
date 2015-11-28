using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedDoubleField : NZazuField<double?>
    {
        public XceedDoubleField(string key) : base(key)
        {
        }

        public override DependencyProperty ContentProperty => DoubleUpDown.ValueProperty;

        public override string Type => "double";

        protected override Control GetValue()
        {
            var control = new DoubleUpDown { ToolTip = Description, Watermark = Hint };
            var formatString = GetSetting("Format");
            if (!string.IsNullOrWhiteSpace(formatString))
                control.FormatString = formatString;
            return control;
        }

        protected override void SetStringValue(string value)
        {
            double result;
            if (!string.IsNullOrWhiteSpace(value) 
                && double.TryParse(value, NumberStyles.Number, FormatProvider, out result))
                Value = result;
            else
                Value = null;
        }

        protected override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString(FormatProvider) : string.Empty;
        }
    }
}