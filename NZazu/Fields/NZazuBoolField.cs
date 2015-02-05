using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NZazu.Fields
{
    class NZazuBoolField : NZazuField<bool?>
    {
        public NZazuBoolField(string key) : base(key)
        {
            Type = "bool";
            ContentProperty = ToggleButton.IsCheckedProperty;
        }

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

        protected override Control GetValue()
        {
            return new CheckBox {Content = Hint, ToolTip = Description, IsChecked = null, IsThreeState = true};
        }
    }
}