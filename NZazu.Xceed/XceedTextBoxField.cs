using System.Windows.Controls;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedTextBoxField : NZazuTextField
    {
        public XceedTextBoxField(string key) : base(key)
        {
        }

        protected override Control GetValue()
        {
            return new WatermarkTextBox { ToolTip = Description, Watermark = Hint};
        }
    }
}