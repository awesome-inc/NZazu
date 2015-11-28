using System.Windows.Controls;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedTextBoxField : NZazuTextField
    {
        internal XceedTextBoxField(string key, 
            string description, string hint) 
            : base(key)
        {
            Description = description;
            Hint = hint;
        }

        public XceedTextBoxField(string key) : this(key, null, null)
        {
        }

        protected override Control GetValue()
        {
            return new WatermarkTextBox {ToolTip = Description, Watermark = Hint};
        }
    }
}