using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedDoubleField : NZazuField<double?>
    {
        public XceedDoubleField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc) { }

        public override DependencyProperty ContentProperty => DoubleUpDown.ValueProperty;

        protected override Control CreateValueControl()
        {
            var control = new DoubleUpDown { ToolTip = Definition.Description, Watermark = Definition.Hint };
            var formatString = Definition.Settings.Get("Format");
            if (!string.IsNullOrWhiteSpace(formatString))
                control.FormatString = formatString;
            return control;
        }

        public override void SetStringValue(string value)
        {
            double result;
            if (!string.IsNullOrWhiteSpace(value)
                && double.TryParse(value, NumberStyles.Number, FormatProvider, out result))
                Value = result;
            else
                Value = null;
        }

        public override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString(FormatProvider) : string.Empty;
        }
    }
}