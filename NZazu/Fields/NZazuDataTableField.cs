using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using NZazu.Contracts;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    public class NZazuDataTableField
        : NZazuField
    {
        #region crappy code to create a new row after tabbing the last field

#pragma warning disable 612
        [Obsolete]
        private Control _lastAddedField;

        private void ChangeLastAddedFieldTo(Control newField)
        {
            if (_lastAddedField != null)
                _lastAddedField.PreviewKeyDown -= LastAddedFieldOnPreviewKeyDown;
            _lastAddedField = newField;
            if (_lastAddedField != null)
                _lastAddedField.PreviewKeyDown += LastAddedFieldOnPreviewKeyDown;
        }

        private void LastAddedFieldOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // double check sender
            if (!ReferenceEquals(sender, _lastAddedField)) return;

            // check shortcut
            var binding = new KeyBinding { Key = System.Windows.Input.Key.Tab };
            if (!binding.Gesture.Matches(sender, e)) return;

            // add rows and handle key
            AddBtnOnClick(sender, e);
        }
#pragma warning restore 612

        #endregion

        private readonly DynamicDataTable _clientControl;
        private readonly IDictionary<string, INZazuWpfField> _fields = new Dictionary<string, INZazuWpfField>();
        private int _tabOrder;

        private Button addBtn;

        public NZazuDataTableField(string key, FieldDefinition definition) : base(key, definition)
        {
            _clientControl = new DynamicDataTable();
            var grid = _clientControl.Grid;

            // header
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24.0) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24) });
            foreach (var field in Definition.Fields ?? Enumerable.Empty<FieldDefinition>())
            {
                // create column with default width
                var width = 135; // default Width
                if (field.Settings != null && field.Settings.ContainsKey("Width")) width = int.Parse(field.Settings["Width"]);
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width) });
                var column = grid.ColumnDefinitions.Count - 1;

                // set the header column ;)
                var lbl = new Label() { Content = field.Prompt, ToolTip = field.Description, Background = Brushes.Silver };
                grid.Children.Add(lbl);
                Grid.SetRow(lbl, 0);
                Grid.SetColumn(lbl, column); // the last one ;)

                // we create a first empty row :)
                var ctrl = new NZazuFieldFactory(new CheckFactory()).CreateField(field);
                ctrl.ValueControl.Name = field.Key + "__1";
                ctrl.ValueControl.TabIndex = _tabOrder++;
                grid.Children.Add(ctrl.ValueControl);
                Grid.SetRow(ctrl.ValueControl, 1);
                Grid.SetColumn(ctrl.ValueControl, column);

                _fields.Add(field.Key + "__1", ctrl);
                ChangeLastAddedFieldTo(ctrl.ValueControl);
            }

            CreateButtonsOn(_clientControl.Panel);
        }

        #region buttons

        private void CreateButtonsOn(Panel panel)
        {
            // add button
            addBtn = new Button { Content = "Add", TabIndex = _tabOrder + 1 };
            addBtn.Click += AddBtnOnClick;
            panel.Children.Add(addBtn);
        }

        private void AddBtnOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var grid = _clientControl.Grid;

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24) });
            var columnCounter = 0;
            foreach (var field in Definition.Fields ?? Enumerable.Empty<FieldDefinition>())
            {
                var rowNo = grid.RowDefinitions.Count - 1; // I added already one ;)

                // we create a first empty row :)
                var ctrl = new NZazuFieldFactory(new CheckFactory()).CreateField(field);
                ctrl.ValueControl.Name = field.Key + "__" + rowNo;
                ctrl.ValueControl.TabIndex = _tabOrder++;
                grid.Children.Add(ctrl.ValueControl);
                Grid.SetRow(ctrl.ValueControl, rowNo);
                Grid.SetColumn(ctrl.ValueControl, columnCounter);

                _fields.Add(ctrl.ValueControl.Name, ctrl);
                ChangeLastAddedFieldTo(ctrl.ValueControl);
                columnCounter++;
            }

            addBtn.TabIndex = _tabOrder + 1;
        }

        #endregion


        public override bool IsEditable => true;

        public override string StringValue
        {
            get
            {
                return GetGridValues();
            }
            set { UpdateGridValues(value); }
        }

        private string GetGridValues()
        {
            var data = _clientControl.Grid.Children
                .Cast<Control>()
                .Where(x =>
                    !string.IsNullOrEmpty(x.Name) &&
                    Definition.Fields.SingleOrDefault(
                        y => y.Key == x.Name.Split(new[] { "__" }, StringSplitOptions.RemoveEmptyEntries)[0]) != null)
                .ToDictionary(
                    child => child.Name,
                    child => child.GetValue(_fields.Single(x => Equals(x.Value.ValueControl, child)).Value.ContentProperty)
                 );

            // todo: extract to interface and implementation and create json project!
            // cf: http://stackoverflow.com/questions/12554186/how-to-serialize-deserialize-to-dictionaryint-string-from-custom-xml-not-us
            var xElem = new XElement(
                "items",
                data.Where(x => x.Value != null).Select(x => new XElement("item", new XAttribute("id", x.Key), new XAttribute("value", x.Value)))
             );
            return xElem.ToString();

        }

        private void UpdateGridValues(string value)
        {
            // clear table??

            var xElem2 = XElement.Parse(value);
            var newDict = xElem2.Descendants("item")
                .ToDictionary(x => (string)x.Attribute("id"), x => (string)x.Attribute("value"));

            var iterations = 0;
            if (newDict.Count > 0)
                iterations = newDict
                    .Max(x => int.Parse(x.Key.Split(new[] { "__" }, StringSplitOptions.RemoveEmptyEntries)[1]));

            while (_clientControl.Grid.RowDefinitions.Count <= iterations)
                AddBtnOnClick(null, null);

            foreach (var field in _fields)
            {
                var kv = newDict.FirstOrDefault(x => x.Key == field.Key);
                if (string.IsNullOrEmpty(kv.Key)) continue;

                field.Value.StringValue = kv.Value;
            }

        }

        public override DependencyProperty ContentProperty => null;
        public override string Type => "datatable";

        protected override Control GetValue()
        {
            return _clientControl;
        }
    }
}