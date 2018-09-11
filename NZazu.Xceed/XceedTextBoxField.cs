using System;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedTextBoxField : NZazuTextField
    {
        public XceedTextBoxField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc) { }

        protected override Control CreateValueControl()
        {
            return new WatermarkTextBox { ToolTip = Definition.Description, Watermark = Definition.Hint };
        }
    }
}