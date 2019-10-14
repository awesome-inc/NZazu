using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.LayoutStrategy
{
    public class StackedLayout : Layout
    {
        public StackedLayout(ControlTemplate errorTemplate = null) : base(errorTemplate)
        {
        }

        public override void DoLayout(
            ContentControl contentControl,
            IEnumerable<INZazuWpfField> fields,
            IResolveLayout resolveLayout = null)
        {
            if (contentControl == null) throw new ArgumentNullException(nameof(contentControl));
            if (fields == null) throw new ArgumentNullException(nameof(fields));

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(5),
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            foreach (var field in fields)
            {
                var labelElement = field.LabelControl;
                var valueElement = field.ValueControl;
                if (labelElement == null && valueElement == null) continue;

                if (labelElement != null)
                {
                    StyleLabel(labelElement);
                    stackPanel.Children.Add(labelElement);
                }

                if (valueElement != null)
                {
                    StyleValue(valueElement);

                    stackPanel.Children.Add(valueElement);
                    SetErrorTemplate(valueElement);
                    ProcessGroupField(resolveLayout, valueElement, field);
                }
            }

            contentControl.Content = stackPanel;
        }
    }
}