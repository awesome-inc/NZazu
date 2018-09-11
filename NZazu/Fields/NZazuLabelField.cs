using System;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    internal class NZazuLabelField : NZazuField
    {
        public NZazuLabelField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            IsEditable = false;
        }

        public override void SetStringValue(string value) { }
        public override string GetStringValue() { return null; }

        public override DependencyProperty ContentProperty => null;

        protected override Control CreateValueControl() { return !string.IsNullOrWhiteSpace(Definition.Description) ? new Label { Content = Definition.Description } : null; }
    }
}