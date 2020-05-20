using System;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuTextField : NZazuField<string>
    {
        private readonly Lazy<Control> _lazyControl;

        public NZazuTextField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            _lazyControl = new Lazy<Control>(() => new TextBox
            {
                ToolTip = Definition.Description,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch

            }, true);
        }

        public override DependencyProperty ContentProperty => TextBox.TextProperty;

        public override void SetValue(string value)
        {
            Value = value;
        }

        public override string GetValue()
        {
            CreateValueControl().GetValue(ContentProperty);
            return Value;
        }

        protected override Control CreateValueControl()
        {
            return _lazyControl.Value;
        }
    }
}