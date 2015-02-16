using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields
{
    public class NZazuIntegerField : NZazuField<int?>
    {
        public NZazuIntegerField(string key) : base(key) { }

        public override string Type { get { return "int"; } }
        public override DependencyProperty ContentProperty { get { return TextBox.TextProperty; } }

        protected override Control GetValue()
        {
            return new TextBox { ToolTip = Description };
        }

        protected override void SetStringValue(string value)
        {
            int result;
            if (!String.IsNullOrWhiteSpace(value) && int.TryParse(value, out result)) 
                Value = result;
            else 
                Value = null;
        }

        protected override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;
        }
    }
}

