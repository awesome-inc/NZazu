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

        public override DependencyProperty ContentProperty => DateTimePickerWithUpdate.ValueProperty;

        protected override Control CreateValueControl()
        {
            var control = new DateTimePickerWithUpdate { ToolTip = Description, Watermark = Hint };

            // set date time format
            DateFormat = GetSetting("Format");
            // ReSharper disable once InvertIf
            if (!string.IsNullOrWhiteSpace(DateFormat))
            {
                control.Format = DateTimeFormat.Custom;
                control.FormatString = DateFormat;
            }

            return control;
        }
    }
}