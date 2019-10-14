using System;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuBoolField : NZazuField<bool?>
    {
        public NZazuBoolField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
        }

        // ReSharper disable once AccessToStaticMemberViaDerivedType
        public override DependencyProperty ContentProperty => CheckBox.IsCheckedProperty;

        public override void SetValue(string value)
        {
            bool b;
            if (!string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out b))
                Value = b;
            else
                Value = null;
        }

        public override string GetValue()
        {
            return Value?.ToString() ?? string.Empty;
        }

        protected override Control CreateValueControl()
        {
            return new CheckBox
            {
                Content = Definition.Hint,
                ToolTip = Definition.Description,
                IsChecked = null,
                IsThreeState = true,
                VerticalContentAlignment = VerticalAlignment.Center
            };
        }
    }
}