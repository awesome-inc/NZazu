using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuBoolField : NZazuField<bool?>
    {
        public NZazuBoolField(string key, FieldDefinition definition) : base(key, definition) { }

        protected override void SetStringValue(string value)
        {
            bool b;
            if (!string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out b))
                Value = b;
            else
                Value = null;
        }

        protected override string GetStringValue()
        {
            return Value?.ToString() ?? string.Empty;
        }

        public override string Type => "bool";
        public override DependencyProperty ContentProperty => ToggleButton.IsCheckedProperty;

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