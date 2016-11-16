using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuTextField : NZazuField<string>
    {
        public NZazuTextField(string key, FieldDefinition definition) : base(key, definition) { }

        protected override void SetStringValue(string value) { Value = value; }
        protected override string GetStringValue() { return Value; }

        public override string Type => "string";

        public override DependencyProperty ContentProperty => TextBox.TextProperty;

        protected override Control GetValue() { return new TextBox { ToolTip = Description }; }
    }
}