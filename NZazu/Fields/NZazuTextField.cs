using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields
{
    public class NZazuTextField : NZazuField<string>
    {
        public NZazuTextField(string key) : base(key)
        {
        }

        protected override void SetStringValue(string value) { Value = value; }
        protected override string GetStringValue() { return Value; }

        public override string Type => "string";

        public override DependencyProperty ContentProperty => TextBox.TextProperty;

        protected override Control GetValue() { return new TextBox { ToolTip = Description }; }
    }
}