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
        public XceedDateTimeField(string key, FieldDefinition definition) : base(key, definition) { }

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