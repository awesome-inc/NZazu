using System.Windows;
using System.Windows.Controls;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;

namespace NZazu.Xceed
{
    public class XceedRichTextField : NZazuTextField
    {
        public const double DefaultHeight = 80.0d;

        public XceedRichTextField(string key)
            : base(key)
        {
        }

        public override DependencyProperty ContentProperty => RichTextBox.TextProperty;

        protected override Control GetValue()
        {
            var control = new RichTextBox
            {
                ToolTip = Description,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MinHeight = DefaultHeight,
                MaxHeight = DefaultHeight,
                TextFormatter = GetFormatter(GetSetting("Format"))
            };

            var showFormatBar = GetSetting<bool>("ShowFormatBar");
            if (showFormatBar == true)
                RichTextBoxFormatBarManager.SetFormatBar(control, new RichTextBoxFormatBar());

            return control;
        }

        private static ITextFormatter GetFormatter(string format)
        {
            switch (format)
            {
                case "plain": return new PlainTextFormatter();
                case "xaml": return new XamlFormatter();
                // ReSharper disable once RedundantCaseLabel
                case "rtf":
                default: return new RtfFormatter();
            }
        }
    }
}