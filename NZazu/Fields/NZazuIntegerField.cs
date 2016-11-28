using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuIntegerField : NZazuField<int?>
    {
        public NZazuIntegerField(FieldDefinition definition) : base(definition) { }

        public override string Type => "int";
        public override DependencyProperty ContentProperty => TextBox.TextProperty;

        protected internal override Control Value
        {
            get
            {
                return new TextBox { ToolTip = Description };
            }
        }

        protected override void SetStringValue(string value)
        {
            int result;
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out result)) 
                Value = result;
            else 
                Value = null;
        }

        protected override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }
    }
}

