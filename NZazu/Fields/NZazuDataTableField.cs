using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    public class NZazuDataTableField
        : NZazuField
    {
        private readonly DynamicDataTable _clientControl;

        public NZazuDataTableField(string key, FieldDefinition definition) : base(key, definition)
        {
            _clientControl = new DynamicDataTable();

            _clientControl.Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24.0) });
            _clientControl.Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24) });
            foreach (var field in Definition.Fields ?? Enumerable.Empty<FieldDefinition>())
            {
                var width = 135;
                if (field.Settings != null && field.Settings.ContainsKey("Width")) width = int.Parse(field.Settings["Width"]);

                _clientControl.Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width) });

                var lbl = new Label() { Content = field.Prompt };
                _clientControl.Grid.Children.Add(lbl);
                Grid.SetRow(lbl, 0);
                Grid.SetColumn(lbl, _clientControl.Grid.ColumnDefinitions.Count - 1);

                var ctrl = new NZazuFieldFactory(new CheckFactory()).CreateField(field);
                _clientControl.Grid.Children.Add(ctrl.ValueControl);
                Grid.SetRow(ctrl.ValueControl, 1);
                Grid.SetColumn(ctrl.ValueControl, _clientControl.Grid.ColumnDefinitions.Count - 1);


            }


        }

        public override bool IsEditable => true;

        public override string StringValue
        {
            get { return _clientControl.ValuesAsJson; }
            set { UpdateGridValues(value); }
        }

        private void UpdateGridValues(string value)
        {

        }

        public override DependencyProperty ContentProperty => DynamicDataTable.ValuesAsJsonProperty;
        public override string Type => "datatable";

        protected override Control GetValue()
        {
            return _clientControl;
        }
    }
}