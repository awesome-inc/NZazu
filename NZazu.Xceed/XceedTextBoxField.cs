using System.Collections.Generic;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedTextBoxField : NZazuTextField
    {
        internal XceedTextBoxField(string key,
            string description, string hint, FieldDefinition definition)
            : base(key, definition)
        {
            Description = description;
            Hint = hint;
        }

        public XceedTextBoxField(string key, FieldDefinition definition) : base(key, definition) { }

        protected override Control GetValue()
        {
            return new WatermarkTextBox { ToolTip = Description, Watermark = Hint };
        }
    }
}