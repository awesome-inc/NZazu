using System.Windows;
using System.Windows.Controls;
using NZazu.FieldFactory;
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

        public override DependencyProperty ContentProperty
        {
            get { return RichTextBox.TextProperty; }
        }

        protected override Control GetValue()
        {
            return new RichTextBox
            {
                ToolTip = Description,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MinHeight = DefaultHeight,
                MaxHeight = DefaultHeight
            };
        }
    }
}