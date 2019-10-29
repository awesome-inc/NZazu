using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Contracts.FormChecks;
using NZazu.EventArgs;
using NZazu.Extensions;
using NZazu.Fields;
using NZazu.Fields.Controls;

namespace NZazu
{
    public class NZazuView : ScrollViewer, INZazuWpfView
    {
        private readonly List<IFormCheck> _checks = new List<IFormCheck>();

        private readonly Dictionary<string, INZazuWpfField> _fields = new Dictionary<string, INZazuWpfField>();
        private INZazuWpfField _lastFocusedElement;

        public NZazuView()
        {
            InitializeComponent();
        }

        public static string FocusOnFieldName => "__focusOn";

        private ContentControl Layout => this;
        public event EventHandler<FieldFocusChangedEventArgs> FieldFocusChanged;

        public void ApplyChanges()
        {
            FormData = GetFieldValues();
        }

        public INZazuWpfField GetField(string key)
        {
            // in case we have a "settings" field.
            if (key.StartsWith("_")) return null;

            if (TryGetField(key, out var field))
                return field;

            throw new KeyNotFoundException();
        }

        public bool TryGetField(string key, out INZazuWpfField field)
        {
            return _fields.TryGetValue(key, out field);
        }

        public Dictionary<string, string> GetFieldValues()
        {
            return _fields
                // lets add the data
                .Where(f => f.Value.IsEditable)
                .Where(f => !string.IsNullOrEmpty(f.Value.GetValue()))
                .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.GetValue()))
                .Select(x => new KeyValuePair<string, string>(x.Key, x.Value))
                // now lets add the control state
                .Concat(_fields
                    .Where(f => !string.IsNullOrEmpty(f.Value.GetValue()))
                    .SelectMany(x => x.Value.GetState()))
                // and don't forget the focus
                .Concat(new[] { new KeyValuePair<string, string>("__focusOn", _lastFocusedElement?.Key) })
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public ValueCheckResult Validate()
        {
            if (_fields?.Values == null) return ValueCheckResult.Success;

            var results = _fields.Values.Select(f => f.Validate())
                .Concat(_checks.Select(f => f.Validate(FormData)))
                .Where(x => x!= null && !x.IsValid)
                .ToArray();

            if (results.Length == 0) return ValueCheckResult.Success;

            foreach (var errorField in _fields.Where(x => x.Value.ValueControl is ErrorPanel))
                ((ErrorPanel)errorField.Value.ValueControl).Errors = results.Select(x => x.Exception.Message);

            return results.Length == 1
                ? results.First()
                : new AggregateValueCheckResult(results);
        }

        public bool TrySetFocusOn(string focusOn = null, bool force = false)
        {
            var safeFocusOn = focusOn ?? (FormData.Values.ContainsKey(FocusOnFieldName)
                                  ? FormData.Values[FocusOnFieldName]
                                  : null);
            if (string.IsNullOrWhiteSpace(safeFocusOn) || !TryGetField(safeFocusOn, out var field)) return false;

            var control = field.ValueControl;
            if (control == null) return false;
            if (force) this.RemoveFocus();

            control.SetFocus();
            control.DelayedFocus();

            return true;
        }

        private void InitializeComponent()
        {
            // cf.: http://compiledexperience.com/blog/posts/using-caliburn-micro-as-a-data-template-selector/
            // we make this tab selectable so we can jump directly into the last selected field
            Layout.Focusable = true;
            Layout.IsTabStop = true;
            Layout.VerticalContentAlignment = VerticalAlignment.Stretch;
            Layout.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            SetHorizontalScrollBarVisibility(Layout, ScrollBarVisibility.Visible);
            SetVerticalScrollBarVisibility(Layout, ScrollBarVisibility.Visible);

            Layout.LostFocus += (s, e) => ApplyChanges();
            Layout.GotKeyboardFocus += (s, e) =>
            {
                var oldFocusedElement = _lastFocusedElement;
                // remember ctrl with focus for state
                var newFocusedElement = GetFocusedControl(e.NewFocus as FrameworkElement);

                // now if I focus on the control, I focus on the last field
                if (Equals(e.NewFocus, Layout))
                    TrySetFocusOn();

                if (newFocusedElement is NZazuDataTableField) return;

                _lastFocusedElement = newFocusedElement;
                OnFieldFocusChanged(new FieldFocusChangedEventArgs(newFocusedElement, oldFocusedElement));
            };

            FieldFactory = new NZazuFieldFactory();
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private INZazuWpfField GetFocusedControl(UIElement focusedSubControl)
        {
            if (focusedSubControl == null) return null;

            var field = _fields.SingleOrDefault(x => Equals(x.Value?.ValueControl, focusedSubControl)).Value;

            if (field != null) return field;

            var parent = VisualTreeHelper.GetParent(focusedSubControl);
            return parent != null
                ? GetFocusedControl(parent as UIElement)
                : null;
        }

        private void UpdateFields(
            FormDefinition formDefinition,
            INZazuWpfFieldFactory fieldFactory,
            IResolveLayout resolveLayout)
        {
            Dispose();
            DisposeChecks();

            // make sure at least the minimum is set for render the layout
            if (formDefinition?.Fields == null) return;

            CreateFields(formDefinition.Fields, fieldFactory);

            if (formDefinition.Checks != null)
                CreateFormChecks(formDefinition.Checks, fieldFactory.Resolve<ICheckFactory>());

            var layout = resolveLayout.Resolve(formDefinition.Layout);

            var parentFields = FormDefinition.Fields.Select(fd => GetField(fd.Key));
            layout.DoLayout(Layout, parentFields, resolveLayout);

            this.SetFieldValues(FormData.Values);

            SetReadOnly(IsReadOnly);
        }

        private void SetReadOnly(bool isReadOnly)
        {
            foreach (var field in _fields.Values)
                field.SetReadOnly(isReadOnly);
        }

        private void CreateFields(IEnumerable<FieldDefinition> formDefinition, INZazuWpfFieldFactory fieldFactory)
        {
            formDefinition.ToList().ForEach(f =>
            {
                // create field
                var field = fieldFactory.CreateField(f);
                AddField(field);
                AddGroupFieldKeys(field as INZazuWpfFieldContainer);
            });
        }

        private void CreateFormChecks(IEnumerable<CheckDefinition> checkDefinition, ICheckFactory checkFactory)
        {
            checkDefinition.ToList().ForEach(f =>
            {
                // create check
                var check = checkFactory.CreateFormCheck(f);
                _checks.Add(check);
            });
        }

        private void AddGroupFieldKeys(INZazuWpfFieldContainer groupField)
        {
            if (groupField?.Fields == null) return;
            foreach (var field in groupField.Fields)
            {
                AddField(field);
                AddGroupFieldKeys(field as INZazuWpfFieldContainer);
            }
        }

        private void AddField(INZazuWpfField field)
        {
            if (field is NZazuDataTableField dataTableField)
                dataTableField.TableFieldFocusChanged += DataTableFieldOnTableFieldFocusChanged;

            _fields.Add(field.Key, field);
        }

        private void Dispose()
        {
            foreach (var field in _fields.Values)
            {
                if (field is NZazuDataTableField dataTableField)
                    dataTableField.TableFieldFocusChanged -= DataTableFieldOnTableFieldFocusChanged;
                field.Dispose();
            }

            _fields.Clear();
        }

        private void DisposeChecks()
        {
            _checks.Clear();
        }

        private void DataTableFieldOnTableFieldFocusChanged(object sender, FieldFocusChangedEventArgs e)
        {
            var oldFocusedElement = _lastFocusedElement;
            _lastFocusedElement = e.NewFocusedElement;

            OnFieldFocusChanged(new FieldFocusChangedEventArgs(_lastFocusedElement, oldFocusedElement,
                e.ParentElement));
        }

        protected virtual void OnFieldFocusChanged(FieldFocusChangedEventArgs e)
        {
            var oldFocusedElement =
                (string.IsNullOrWhiteSpace(e.OldFocusedElement?.ValueControl?.Name)
                    ? e.OldFocusedElement?.Key
                    : e.OldFocusedElement?.ValueControl?.Name) ?? "empty";
            var newFocusedElement =
                (string.IsNullOrWhiteSpace(e.NewFocusedElement?.ValueControl?.Name)
                    ? e.NewFocusedElement?.Key
                    : e.NewFocusedElement?.ValueControl?.Name) ?? "empty";
            Trace.WriteLine($"Field focus changed from <{oldFocusedElement}> to <{newFocusedElement}>");

            FieldFocusChanged?.Invoke(this, e);
        }

        #region dependency properties

        // ############# FormDefinition

        public static readonly DependencyProperty FormDefinitionProperty = DependencyProperty.Register(
            "FormDefinition", typeof(FormDefinition), typeof(NZazuView),
            new PropertyMetadata(default(FormDefinition), FormDefinitionChanged));

        public FormDefinition FormDefinition
        {
            get => (FormDefinition)GetValue(FormDefinitionProperty);
            set => SetValue(FormDefinitionProperty, value);
        }

        private static void FormDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var formDefinition = (FormDefinition)e.NewValue;
            view.UpdateFields(formDefinition, view.FieldFactory, view.ResolveLayout);
        }

        // ############# FieldFactory

        public static readonly DependencyProperty FieldFactoryProperty = DependencyProperty.Register(
            "FieldFactory", typeof(INZazuWpfFieldFactory), typeof(NZazuView),
            new PropertyMetadata(new NZazuFieldFactory(), FieldFactoryChanged, FieldFactoryCoerceCallback));

        public INZazuWpfFieldFactory FieldFactory
        {
            get => (INZazuWpfFieldFactory)GetValue(FieldFactoryProperty);
            set
            {
                if (value == null)
                    value = new NZazuFieldFactory();
                value.Use(this as INZazuWpfView);
                SetValue(FieldFactoryProperty, value);
            }
        }

        private static void FieldFactoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var fieldFactory = (INZazuWpfFieldFactory)e.NewValue;
            fieldFactory.Use<INZazuWpfView>(view);
            view.UpdateFields(view.FormDefinition, fieldFactory, view.ResolveLayout);
        }

        private static object FieldFactoryCoerceCallback(DependencyObject d, object baseValue)
        {
            var view = (NZazuView)d;
            var fieldFactory = (INZazuWpfFieldFactory)baseValue;
            return fieldFactory ?? view.FieldFactory;
        }

        public static readonly DependencyProperty ResolveLayoutProperty = DependencyProperty.Register(
            "ResolveLayout", typeof(IResolveLayout), typeof(NZazuView),
            new PropertyMetadata(new ResolveLayout(), ResolveLayoutChanged, ResolveLayoutCoerceCallback));

        public IResolveLayout ResolveLayout
        {
            get => (IResolveLayout)GetValue(ResolveLayoutProperty);
            set => SetValue(ResolveLayoutProperty, value);
        }

        private static void ResolveLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var layoutStrategy = (IResolveLayout)e.NewValue;
            view.UpdateFields(view.FormDefinition, view.FieldFactory, layoutStrategy);
        }

        private static object ResolveLayoutCoerceCallback(DependencyObject d, object baseValue)
        {
            var view = (NZazuView)d;
            var layout = (IResolveLayout)baseValue;
            return layout ?? view.ResolveLayout;
        }

        public static readonly DependencyProperty FormDataProperty = DependencyProperty.Register(
            "FormData", typeof(FormData), typeof(NZazuView),
            new PropertyMetadata(new FormData(), FormDataChanged));

        private static void FormDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (INZazuWpfView)d;
            var fieldValues = (FormData)e.NewValue;
            view.SetFieldValues(fieldValues.Values);
        }

        public FormData FormData
        {
            get => (FormData)GetValue(FormDataProperty);
            set => SetValue(FormDataProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(NZazuView), new PropertyMetadata(default(bool), IsReadOnlyChanged));

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var isReadOnly = (bool)e.NewValue;
            view.SetReadOnly(isReadOnly);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        #endregion
    }
}