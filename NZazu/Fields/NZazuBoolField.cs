using System.Windows.Controls;

namespace NZazu.Fields
{
    class NZazuBoolField : NZazuField
    {
        public NZazuBoolField(string key) : base(key)
        {
            Type = "bool";
        }

        protected override Control GetValue()
        {
            return new CheckBox {Content = Hint, ToolTip = Description};
        }
    }
}