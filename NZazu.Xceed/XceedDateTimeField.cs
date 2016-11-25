using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedDateTimeField : NZazuDateField
    {
        public XceedDateTimeField(FieldDefinition definition) : base(definition) { }

        public override DependencyProperty ContentProperty => DateTimePicker.ValueProperty;

        protected override Control GetValue()
        {
            var control = new DateTimePicker { ToolTip = Description, Watermark = Hint };
            DateFormat = GetSetting("Format");
            if (!string.IsNullOrWhiteSpace(DateFormat))
            {
                control.Format = DateTimeFormat.Custom;
                control.FormatString = DateFormat;
            }
            return control;
        }
    }
}