using System.Collections.Generic;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedTextBoxField : NZazuTextField
    {
        internal XceedTextBoxField(string description, string hint, FieldDefinition definition)
            : base(definition)
        {
            Description = description;
            Hint = hint;
        }

        public XceedTextBoxField(FieldDefinition definition) : base(definition) { }

        protected internal override Control Value
        {
            get
            {
                return new WatermarkTextBox { ToolTip = Description, Watermark = Hint };
            }
        }
    }
}