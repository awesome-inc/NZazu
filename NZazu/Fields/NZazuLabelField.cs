using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    internal class NZazuLabelField : NZazuField
    {
        public NZazuLabelField(FieldDefinition definition) : base(definition) { }

        public override string Type => "label";

        public override bool IsEditable => false;

        protected override void SetStringValue(string value) { }
        protected override string GetStringValue() { return null; }

        public override DependencyProperty ContentProperty => null;

        protected override Control CreateValueControl() { return !string.IsNullOrWhiteSpace(Description) ? new Label { Content = Description } : null; }
    }
}