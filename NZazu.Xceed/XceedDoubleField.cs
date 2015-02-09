using System;
using System.Windows;
using System.Windows.Controls;
using NZazu.FieldFactory;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedDoubleField : NZazuField<double?>
    {
        public XceedDoubleField(string key) : base(key)
        {
        }

        public override DependencyProperty ContentProperty
        {
            get { return DoubleUpDown.ValueProperty; }
        }

        public override string Type { get { return "double"; } }

        protected override Control GetValue()
        {
            return new DoubleUpDown {ToolTip = Description, Watermark = Hint, FormatString = GetSetting("Format")};
        }

        protected override void SetStringValue(string value)
        {
            try
            {
                var date = double.Parse(value, FormatProvider);
                Value = date;
            }
            catch (FormatException)
            {
                Value = null;
            }
        }

        protected override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString(FormatProvider) : String.Empty;
        }
    }
}