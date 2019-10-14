using System;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedDateTimeField : NZazuDateField
    {
        public XceedDateTimeField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
        }

        public override DependencyProperty ContentProperty => DateTimePickerWithUpdate.ValueProperty;

        protected override Control CreateValueControl()
        {
            var control = new DateTimePickerWithUpdate {ToolTip = Definition.Description, Watermark = Definition.Hint};

            // set date time format
            DateFormat = Definition.Settings.Get(nameof(control.Format));
            // ReSharper disable once InvertIf
            if (!string.IsNullOrWhiteSpace(DateFormat))
            {
                control.Format = DateTimeFormat.Custom;
                control.FormatString = DateFormat;

                if (Definition.Settings.ContainsKey(nameof(control.Format)))
                    Definition.Settings[nameof(control.Format)] =
                        Enum.GetName(typeof(DateTimeFormat), DateTimeFormat.Custom);
            }

            return control;
        }
    }
}