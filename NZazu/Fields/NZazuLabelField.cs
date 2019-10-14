using System;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    internal class NZazuLabelField : NZazuField
    {
        public NZazuLabelField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            IsEditable = false;
        }

        public override DependencyProperty ContentProperty => null;

        public override void SetValue(string value)
        {
        }

        public override string GetValue()
        {
            return null;
        }

        public override ValueCheckResult Validate()
        {
            return ValueCheckResult.Success;
        }

        protected override Control CreateValueControl()
        {
            return !string.IsNullOrWhiteSpace(Definition.Description)
                ? new Label {Content = Definition.Description}
                : null;
        }
    }
}