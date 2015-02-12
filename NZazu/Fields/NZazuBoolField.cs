using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NZazu.Fields
{
    public class NZazuBoolField : NZazuField<bool?>
    {
        public NZazuBoolField(string key) : base(key) { }

        protected override void SetStringValue(string value)
        {
            bool b;
            if (bool.TryParse(value, out b))
                Value = b;
            else
                Value = null;
        }

        protected override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString() : String.Empty;
        }

        public override string Type { get { return "bool"; } }
        public override DependencyProperty ContentProperty { get { return ToggleButton.IsCheckedProperty; } }

        protected override Control GetValue()
        {
            return new CheckBox
            {
                Content = Hint, ToolTip = Description, 
                IsChecked = null, IsThreeState = true,
                VerticalContentAlignment = VerticalAlignment.Center
            };
        }
    }
}