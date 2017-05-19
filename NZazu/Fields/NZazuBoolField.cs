using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuBoolField : NZazuField<bool?>
    {
        public NZazuBoolField(FieldDefinition definition) : base(definition) { }

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
        // ReSharper disable once AccessToStaticMemberViaDerivedType
        public override DependencyProperty ContentProperty => CheckBox.IsCheckedProperty;

        protected override Control CreateValueControl()
        {
            return new CheckBox
            {
                Content = Hint,
                ToolTip = Description,
                IsChecked = null,
                IsThreeState = true,
                VerticalContentAlignment = VerticalAlignment.Center
            };
        }
    }
}