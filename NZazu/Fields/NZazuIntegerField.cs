using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuIntegerField : NZazuField<int?>
    {
        public NZazuIntegerField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
        }

        public override DependencyProperty ContentProperty => TextBox.TextProperty;

        protected internal override Binding DecorateBinding(Binding binding)
        {
            binding.TargetNullValue = string.Empty;
            return base.DecorateBinding(binding);
        }

        protected override Control CreateValueControl()
        {
            return new TextBox {ToolTip = Definition.Description};
        }

        public override void SetValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out var result))
                Value = result;
            else
                Value = null;
        }

        public override string GetValue()
        {
            return Value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}