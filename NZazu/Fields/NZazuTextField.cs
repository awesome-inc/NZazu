using System;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuTextField : NZazuField<string>
    {
        public NZazuTextField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc) { }

        public override void SetValue(string value)
        {
            Value = value;
        }

        public override string GetValue()
        {
            CreateValueControl().GetValue(ContentProperty);
            return Value;
        }

        public override DependencyProperty ContentProperty => TextBox.TextProperty;

        protected override Control CreateValueControl()
        {
            var control = new TextBox { ToolTip = Definition.Description };
            return control;
        }
    }
}