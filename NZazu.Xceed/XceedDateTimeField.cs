using System.Windows.Controls;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedDateTimeField : NZazuDateField
    {
        public XceedDateTimeField(string key) : base(key)
        {
        }

        protected override Control GetValue()
        {
            var control = new DateTimePicker { ToolTip = Description, Watermark = Hint };
            if (Settings != null && Settings.ContainsKey("Format"))
                control.FormatString = Settings["Format"];
            return control;
        }
    }
}