using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FontAwesome.Sharp;
using NEdifis.Attributes;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.EventArgs;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    public class NZazuDataTableField
        : NZazuField
    {
        private readonly INZazuWpfFieldFactory _factory;

        private readonly Dictionary<string, INZazuWpfField> _fields = new Dictionary<string, INZazuWpfField>();
        private readonly INZazuTableDataSerializer _serializer;

        private Button _addBtn;
        private Button _delBtn;

        private INZazuWpfField _lastFocusedElement;
        private int _tabOrder;

        public NZazuDataTableField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            _serializer = (INZazuTableDataSerializer) serviceLocatorFunc(typeof(INZazuTableDataSerializer));
            _factory = (INZazuWpfFieldFactory) serviceLocatorFunc(typeof(INZazuWpfFieldFactory));
        }

        public override DependencyProperty ContentProperty => null;
        public event EventHandler<FieldFocusChangedEventArgs> TableFieldFocusChanged;

        public override void SetValue(string value)
        {
            UpdateGridValues(value);
        }

        public override string GetValue()
        {
            return GetGridValues();
        }

        private string GetGridValues()
        {
            var data = ((DynamicDataTable) ValueControl).LayoutGrid.Children
                .Cast<Control>()
                .Where(x =>
                    !string.IsNullOrEmpty(x.Name) &&
                    Definition.Fields.SingleOrDefault(
                        y => y.Key == x.Name.Split(new[] {"__"}, StringSplitOptions.RemoveEmptyEntries)[0]) != null)
                .ToDictionary(
                    child => child.Name,
                    child => _fields.Single(x => Equals(x.Value.ValueControl, child)).Value.GetValue()
                );

            return _serializer.Serialize(data);
        }

        private void UpdateGridValues(string value)
        {
            var newDict = new Dictionary<string, string>();
            try
            {
                newDict.MergeWith(_serializer.Deserialize(value));
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(
                    "[NZazu.NZazuDataTable.UpdateGridValues] data cannot be parsed. therefore the list will be empty. {0}",
                    ex.Message);
            }

            var iterations = 0;
            if (newDict.Count > 0)
                iterations = newDict
                    .Max(x => int.Parse(x.Key.Split(new[] {"__"}, StringSplitOptions.RemoveEmptyEntries)[1]));

            var layout = ((DynamicDataTable) ValueControl).LayoutGrid;
            if (iterations > 0)
                while (layout.RowDefinitions.Count > iterations + 1)
                {
                    var lastField = ((DynamicDataTable) ValueControl).LayoutGrid.Children.Cast<UIElement>()
                        .First(x => Grid.GetRow(x) == layout.RowDefinitions.Count - 1 &&
                                    Grid.GetColumn(x) == layout.ColumnDefinitions.Count - 1);
                    DeleteRow(lastField);
                }

            while (layout.RowDefinitions.Count <= iterations)
                AddNewRow(layout);

            foreach (var field in _fields)
            {
                var kv = newDict.FirstOrDefault(x => x.Key == field.Key);
                if (string.IsNullOrEmpty(kv.Key)) continue;

                field.Value.SetValue(kv.Value);
            }

            if (layout.RowDefinitions.Count == 1)
                AddNewRow(layout, 1);
        }

        protected override Control CreateValueControl()
        {
            var result = new DynamicDataTable();
            CreateClientControlsOn(result.LayoutGrid);
            // somewhere here i need to attach the behaviour
            CreateButtonsOn(result.ButtonPanel);
            return result;
        }

        private void CreateClientControlsOn(Grid grid)
        {
            // header
            grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(24.0)});
            foreach (var field in Definition.Fields ?? Enumerable.Empty<FieldDefinition>())
            {
                // create column with default width
                var width = 135; // default Width
                if (field.Settings != null && field.Settings.ContainsKey("Width"))
                    width = int.Parse(field.Settings["Width"]);
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(width)});
                var column = grid.ColumnDefinitions.Count - 1;

                // set the header column ;)
                var lbl = new Label
                {
                    Content = field.Prompt,
                    ToolTip = field.Description,
                    Background = Brushes.Silver,
                    FontWeight = FontWeights.Bold
                };
                if (string.IsNullOrEmpty(Definition.Description)) lbl.ToolTip = field.Prompt;
                grid.Children.Add(lbl);
                Grid.SetRow(lbl, 0);
                Grid.SetColumn(lbl, column); // the last one ;)
            }

            AddNewRow(grid);
        }

        #region create _clientControl

        private void CreateButtonsOn(Panel panel)
        {
            // add button
            _addBtn = new Button
            {
                Content = new IconBlock {Icon = IconChar.PlusCircle, Foreground = Brushes.DarkGreen},
                TabIndex = _tabOrder + 1,
                FontFamily = new FontFamily("/FontAwesome.Sharp;component/fonts/#FontAwesome"),
                Width = 24
            };
            _addBtn.Click += AddBtnOnClick;
            panel.Children.Add(_addBtn);

            // del button
            _delBtn = new Button
            {
                Content = new IconBlock {Icon = IconChar.MinusCircle, Foreground = Brushes.DarkRed},
                TabIndex = _tabOrder + 2,
                FontFamily = new FontFamily("/FontAwesome.Sharp;component/fonts/#FontAwesome"),
                Width = 24
            };
            _delBtn.Click += DelBtnOnClick;
            panel.Children.Add(_delBtn);
        }

        #endregion

        public override ValueCheckResult Validate()
        {
            //var result = base.Validate();

            IList<ValueCheckResult> result = new List<ValueCheckResult>();

            foreach (var field in _fields)
            {
                if (!field.Value.IsEditable) continue;

                var tempResult = field.Value.Validate();
                if (!tempResult.IsValid)
                    result.Add(new ValueCheckResult(false, tempResult.Exception));
            }

            switch (result.Count)
            {
                case 0:
                    return ValueCheckResult.Success;
                case 1:
                    return result.First();
                default:
                    return new ValueCheckResult(
                        result.Any(x => x.IsValid),
                        new Exception(string.Concat(result.Select(x => x.ToString()).ToArray())));
            }
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var field in _fields.Values)
            {
                RemoveShortcutsFrom(field.ValueControl);
                RemoveFocusAwarenessFrom(field.ValueControl);

                field.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void OnTableFieldFocusChanged(FieldFocusChangedEventArgs e)
        {
            TableFieldFocusChanged?.Invoke(this, e);
        }

        #region crappy code to create a new row after tabbing the last field

#pragma warning disable 612
        [Obsolete] private UIElement _lastAddedField;

        private void ChangeLastAddedFieldTo(UIElement newField)
        {
            if (_lastAddedField != null)
                _lastAddedField.PreviewKeyDown -= LastAddedFieldOnPreviewKeyDown;
            _lastAddedField = newField;
            if (_lastAddedField != null)
                _lastAddedField.PreviewKeyDown += LastAddedFieldOnPreviewKeyDown;
        }

        [ExcludeFromCodeCoverage]
        private void LastAddedFieldOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // double check sender
            if (!ReferenceEquals(sender, _lastAddedField)) return;

            // check shortcut
            var binding = new KeyBinding {Key = System.Windows.Input.Key.Tab};
            if (!binding.Gesture.Matches(sender, e)) return;

            // add rows and handle key
            AddNewRow(((DynamicDataTable) ValueControl).LayoutGrid);
        }
#pragma warning restore 612

        #endregion

        #region shortcuts and insert/delete line

        private readonly KeyBinding _addRowAboveShortcut1 = new KeyBinding
            {Key = System.Windows.Input.Key.OemPlus, Modifiers = ModifierKeys.Control};

        private readonly KeyBinding _addRowAboveShortcut2 = new KeyBinding
            {Key = System.Windows.Input.Key.Insert, Modifiers = ModifierKeys.Control};

        private readonly KeyBinding _deleteRowShortcut1 = new KeyBinding
            {Key = System.Windows.Input.Key.OemMinus, Modifiers = ModifierKeys.Control};

        private readonly KeyBinding _deleteRowShortcut2 = new KeyBinding
            {Key = System.Windows.Input.Key.Delete, Modifiers = ModifierKeys.Control};

        private void AttachShortcutsTo(UIElement ctrl)
        {
            if (ctrl == null) throw new ArgumentNullException(nameof(ctrl));
            ctrl.PreviewKeyDown += ValueControl_PreviewKeyDown;
        }

        private void RemoveShortcutsFrom(UIElement ctrl)
        {
            if (ctrl == null) throw new ArgumentNullException(nameof(ctrl));
            ctrl.PreviewKeyDown -= ValueControl_PreviewKeyDown;
        }

        private void AttachFocusAwarenessTo(UIElement ctrl)
        {
            if (ctrl == null) throw new ArgumentNullException(nameof(ctrl));
            ctrl.GotKeyboardFocus += ValueControlOnGotKeyboardFocus;
        }

        private void RemoveFocusAwarenessFrom(UIElement ctrl)
        {
            if (ctrl == null) throw new ArgumentNullException(nameof(ctrl));
            ctrl.GotKeyboardFocus -= ValueControlOnGotKeyboardFocus;
        }

        [ExcludeFromCodeCoverage]
        [Because("I need to find out how to test shortcuts with modifier keys")]
        private void ValueControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_addRowAboveShortcut1.Gesture.Matches(sender, e) || _addRowAboveShortcut2.Gesture.Matches(sender, e))
                AddRowAbove(sender);
            if (_deleteRowShortcut1.Gesture.Matches(sender, e) || _deleteRowShortcut2.Gesture.Matches(sender, e))
                DeleteRow(sender);
        }

        [ExcludeFromCodeCoverage]
        [Because("I need to find out how to test focus awareness")]
        private void ValueControlOnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is Control control)
                if (_fields.ContainsKey(control.Name))
                {
                    var oldFocusedElement = _lastFocusedElement;
                    // remember ctrl with focus for state
                    var newFocusedElement = _fields[control.Name];

                    _lastFocusedElement = newFocusedElement;

                    OnTableFieldFocusChanged(new FieldFocusChangedEventArgs(newFocusedElement, oldFocusedElement,
                        this));
                }
        }

        internal void DeleteRow(object sender)
        {
            if (!(sender is Control ctrl)) return;

            var row = Grid.GetRow(ctrl);
            if (row == 0) return; // cannot delete header
            if (((DynamicDataTable) ValueControl).LayoutGrid.RowDefinitions.Count <= 2)
                return; // cannot delete last input row

            var controlsToDelete = ((DynamicDataTable) ValueControl)
                .LayoutGrid.Children.Cast<UIElement>()
                .Where(e => Grid.GetRow(e) == row)
                .ToList();
            controlsToDelete.ForEach(delegate(UIElement control)
            {
                RemoveShortcutsFrom(control);
                RemoveFocusAwarenessFrom(control);

                ((DynamicDataTable) ValueControl).LayoutGrid.Children.Remove(control);
                var elemToDel = _fields.First(x => Equals(x.Value.ValueControl, control));
                elemToDel.Value.Dispose();
                _fields.Remove(elemToDel.Key);
            });

            RecalculateFieldKeys();

            var row2Delete = ((DynamicDataTable) ValueControl).LayoutGrid.RowDefinitions.Count - 1;
            ((DynamicDataTable) ValueControl).LayoutGrid.RowDefinitions.RemoveAt(row2Delete);

            // lets assume the last control in _fields is the lastAddedField
            ChangeLastAddedFieldTo(((DynamicDataTable) ValueControl).LayoutGrid.Children.Cast<UIElement>()
                .First(x => Grid.GetRow(x) == ((DynamicDataTable) ValueControl).LayoutGrid.RowDefinitions.Count - 1 &&
                            Grid.GetColumn(x) ==
                            ((DynamicDataTable) ValueControl).LayoutGrid.ColumnDefinitions.Count - 1));
        }

        // renumber all the fields so they are back in order again
        private void RecalculateFieldKeys()
        {
            var newFields = new Dictionary<string, INZazuWpfField>();
            var lastIndex = string.Empty;
            var index = 0;
            foreach (var field in _fields.OrderBy(x => int.Parse(x.Key.Split(new[] {"__"}, StringSplitOptions.None)[1]))
            )
            {
                var splits = field.Key.Split(new[] {"__"}, StringSplitOptions.None);

                if (lastIndex != splits[1])
                {
                    index++;
                    lastIndex = splits[1];
                }

                var newKey = splits[0] + "__" + index;

                // set new field and move the control to the current row (index)
                field.Value.ValueControl.Name = newKey;
                newFields.AddOrReplace(newKey, field.Value);
                Grid.SetRow(field.Value.ValueControl, index);
            }

            _fields.Clear();
            newFields.ToList().ForEach(x => _fields.AddOrReplace(x.Key, x.Value));
        }

        internal void AddRowAbove(object sender)
        {
            if (!(sender is Control ctrl)) return;

            var row = Grid.GetRow(ctrl);
            if (row == 0) return; // cannot insert above header

            var fieldsAboveInsert =
                _fields.Where(x => int.Parse(x.Key.Split(new[] {"__"}, StringSplitOptions.None)[1]) < row).ToArray();
            var fieldsBelowInsert =
                _fields.Where(x => int.Parse(x.Key.Split(new[] {"__"}, StringSplitOptions.None)[1]) >= row).ToArray();
            var fieldsBelowWithNewName = new Dictionary<string, INZazuWpfField>();

            // now we need to move the below fields to the next row (a little like Hilberts Hotel)
            var layout = ((DynamicDataTable) ValueControl).LayoutGrid;
            layout.RowDefinitions.Add(new RowDefinition {Height = new GridLength(24)});
            foreach (var field in fieldsBelowInsert.OrderBy(x =>
                int.Parse(x.Key.Split(new[] {"__"}, StringSplitOptions.None)[1])))
            {
                // move to next row
                var currentRow = Grid.GetRow(field.Value.ValueControl);
                Grid.SetRow(field.Value.ValueControl, currentRow + 1);

                // change fields index and control name (for serialization)
                var splits = field.Key.Split(new[] {"__"}, StringSplitOptions.None);
                var newKey = splits[0] + "__" + (currentRow + 1);
                field.Value.ValueControl.Name = newKey;
                fieldsBelowWithNewName.AddOrReplace(newKey, field.Value);
            }

            // ok, lets fill the  fields
            _fields.Clear();
            fieldsAboveInsert.ToList().ForEach(x => _fields.Add(x.Key, x.Value));
            AddNewRow(layout, row); // this adds the new controls to the field list
            fieldsBelowWithNewName.ToList().ForEach(x => _fields.Add(x.Key, x.Value));
        }

        #endregion

        #region buttons

        [ExcludeFromCodeCoverage]
        private void AddBtnOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var layout = ((DynamicDataTable) ValueControl).LayoutGrid;
            AddNewRow(layout);
        }

        [ExcludeFromCodeCoverage]
        private void DelBtnOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var lastField = ((DynamicDataTable) ValueControl).LayoutGrid.Children.Cast<UIElement>()
                .First(x => Grid.GetRow(x) == ((DynamicDataTable) ValueControl).LayoutGrid.RowDefinitions.Count - 1 &&
                            Grid.GetColumn(x) ==
                            ((DynamicDataTable) ValueControl).LayoutGrid.ColumnDefinitions.Count - 1);

            DeleteRow(lastField);
        }

        private void AddNewRow(Grid grid, int row = -1)
        {
            int rowNo;
            if (row == -1) // we add a row at the end
            {
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(24)});
                rowNo = grid.RowDefinitions.Count - 1;
            }
            else
            {
                rowNo = row;
            }

            var columnCounter = 0;
            foreach (var field in (Definition.Fields ?? Enumerable.Empty<FieldDefinition>()).ToArray())
            {
                // we create a first empty row :)
                var ctrl = _factory.CreateField(field, rowNo);
                ctrl.ValueControl.Name = field.Key + "__" + rowNo;
                ctrl.ValueControl.TabIndex = _tabOrder++;

                AttachShortcutsTo(ctrl.ValueControl);
                AttachFocusAwarenessTo(ctrl.ValueControl);

                grid.Children.Add(ctrl.ValueControl);
                Grid.SetRow(ctrl.ValueControl, rowNo);
                Grid.SetColumn(ctrl.ValueControl, columnCounter);

                _fields.AddOrReplace(ctrl.ValueControl.Name, ctrl);
                if (row == -1) // only if added new row
                    ChangeLastAddedFieldTo(ctrl.ValueControl);

                columnCounter++;
            }

            if (_addBtn != null)
                _addBtn.TabIndex = _tabOrder + 1;
            if (_delBtn != null)
                _delBtn.TabIndex = _tabOrder + 2;
        }

        #endregion
    }
}