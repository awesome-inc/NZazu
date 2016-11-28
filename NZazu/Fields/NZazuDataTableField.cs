using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    public class NZazuDataTableField
        : NZazuField
        , IRequireFactory
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

        public INZazuWpfFieldFactory FieldFactory { get; set; }

        private DynamicDataTable _clientControl;
        private readonly IDictionary<string, INZazuWpfField> _fields = new Dictionary<string, INZazuWpfField>();
        private int _tabOrder;

        private Button _addBtn;

        public NZazuDataTableField(FieldDefinition definition) : base(definition) { }

        #region buttons

        internal void AddBtnOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var grid = _clientControl.Grid;

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24) });
            var columnCounter = 0;
            foreach (var field in Definition.Fields ?? Enumerable.Empty<FieldDefinition>())
            {
                var rowNo = grid.RowDefinitions.Count - 1; // I added already one ;)

                // we create a first empty row :)
                var ctrl = FieldFactory.CreateField(field);
                ctrl.ValueControl.Name = field.Key + "__" + rowNo;
                ctrl.ValueControl.TabIndex = _tabOrder++;
                grid.Children.Add(ctrl.ValueControl);
                Grid.SetRow(ctrl.ValueControl, rowNo);
                Grid.SetColumn(ctrl.ValueControl, columnCounter);

                _fields.Add(ctrl.ValueControl.Name, ctrl);
                ChangeLastAddedFieldTo(ctrl.ValueControl);
                columnCounter++;
            }

            _addBtn.TabIndex = _tabOrder + 1;
        }

        #endregion

        public override bool IsEditable => false;

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
                    child => _fields.Single(x => Equals(x.Value.ValueControl, child)).Value.StringValue
                 );

            return FieldFactory.Serializer.Serialize(data);
        }

        private void UpdateGridValues(string value)
        {
            var newDict = FieldFactory.Serializer.Deserialize(value);

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

        protected internal override Control Value
        {
            get
            {
                if (_clientControl != null) return _clientControl;

                _clientControl = new DynamicDataTable();
                CreateClientControlsOn(_clientControl.Grid);
                // somewhere here i need to attach the behaviour
                CreateButtonsOn(_clientControl.Panel);
                return _clientControl;
            }
        }

        private void CreateClientControlsOn(Grid grid)
        {
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
                var lbl = new Label()
                {
                    Content = field.Prompt,
                    ToolTip = field.Description,
                    Background = Brushes.Silver,
                    FontWeight = FontWeights.Bold
                };
                if (string.IsNullOrEmpty(Description)) lbl.ToolTip = field.Prompt;
                grid.Children.Add(lbl);
                Grid.SetRow(lbl, 0);
                Grid.SetColumn(lbl, column); // the last one ;)

                // we create a first empty row :)
                var ctrl = FieldFactory.CreateField(field);
                ctrl.ValueControl.Name = field.Key + "__1";
                ctrl.ValueControl.TabIndex = _tabOrder++;
                grid.Children.Add(ctrl.ValueControl);
                Grid.SetRow(ctrl.ValueControl, 1);
                Grid.SetColumn(ctrl.ValueControl, column);

                _fields.Add(field.Key + "__1", ctrl);
                ChangeLastAddedFieldTo(ctrl.ValueControl);
            }
        }

        #region create _clientControl

        private void CreateButtonsOn(Panel panel)
        {
            // add button
            _addBtn = new Button { Content = "Add", TabIndex = _tabOrder + 1 };
            _addBtn.Click += AddBtnOnClick;
            panel.Children.Add(_addBtn);
        }

        #endregion

        public override ValueCheckResult Validate()
        {
            var result = base.Validate();

            foreach (var field in _fields)
            {
                if (!field.Value.IsEditable) continue;

                var iterRes = field.Value.Validate();
                if (!iterRes.IsValid)
                    result = new ValueCheckResult(false, iterRes.Error);
            }
            return result;
        }
    }
}