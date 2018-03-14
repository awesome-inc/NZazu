using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuTextField : NZazuField<string>
    {
        private TextBox _control;
        private readonly object _lockObj = new object();

        public NZazuTextField(FieldDefinition definition) : base(definition) { }

        protected override void SetStringValue(string value)
        {
            Value = value;
        }
        protected override string GetStringValue()
        {
            CreateValueControl().GetValue(ContentProperty);
            return Value;
        }

        public override string Type => "string";

        public override DependencyProperty ContentProperty => TextBox.TextProperty;

        protected override Control CreateValueControl()
        {
            lock (_lockObj)
            {
                if (_control == null)
                    _control = new TextBox { ToolTip = Description };
            }

            return _control;
        }
    }
}