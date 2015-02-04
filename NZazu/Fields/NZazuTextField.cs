using System.Windows.Controls;

namespace NZazu.Fields
{
    class NZazuTextField : NZazuField
    {
        public NZazuTextField(string key) : base(key)
        {
            Type = "string";
            ContentProperty = TextBox.TextProperty;
        }

        protected override Control GetValue()
        {
            return new TextBox
            {
                //Text = Hint, 
                ToolTip = Description
            };
        }
    }
}