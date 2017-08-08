using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuIntegerField : NZazuField<int?>
    {
        public NZazuIntegerField(FieldDefinition definition) : base(definition) { }

        public override string Type => "int";
        public override DependencyProperty ContentProperty => TextBox.TextProperty;

        protected override Binding DecorateBinding(Binding binding)
        {
            binding.TargetNullValue = string.Empty;
            return base.DecorateBinding(binding);
        }

        protected override Control CreateValueControl()
        {
            return new TextBox { ToolTip = Description };
        }

        protected override void SetStringValue(string value)
        {
            int result;
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out result)) 
                Value = result;
            else 
                Value = null;
        }

        protected override string GetStringValue()
        {
            return Value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}

