using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.LayoutStrategy
{
    public class GridLayout : Layout
    {
        public GridLayout(ControlTemplate errorTemplate = null)
            : base(errorTemplate)
        {
        }

        public override void DoLayout(ContentControl contentControl, IEnumerable<INZazuWpfField> fields,
            IResolveLayout resolveLayout = null)
        {
            if (contentControl == null) throw new ArgumentNullException(nameof(contentControl));
            if (fields == null) throw new ArgumentNullException(nameof(fields));

            var grid = new Grid { Margin = new Thickness(5) };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var row = 0;
            foreach (var field in fields)
            {
                var labelElement = field.LabelControl;
                var valueElement = field.ValueControl;
                if (labelElement == null && valueElement == null) continue;

                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                if (labelElement != null)
                {
                    StyleLabel(labelElement);

                    Grid.SetColumn(labelElement, 0);
                    Grid.SetRow(labelElement, row);
                    grid.Children.Add(labelElement);
                }

                if (valueElement != null)
                {
                    StyleValue(valueElement);

                    Grid.SetColumn(valueElement, 1);
                    Grid.SetRow(valueElement, row);
                    grid.Children.Add(valueElement);

                    SetErrorTemplate(valueElement);
                    ProcessGroupField(resolveLayout, valueElement, field);
                }

                row++;
            }

            contentControl.Content = grid;
        }
    }
}