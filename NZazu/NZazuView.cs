using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Extensions;
using NZazu.FieldBehavior;
using NZazu.Fields;
using NZazu.Serializer;

namespace NZazu
{
    public class NZazuView : ScrollViewer, INZazuWpfView
    {
        public static string FocusOnFieldName => "__focusOn";

        #region dependency properties

        // ############# FormDefinition

        public static readonly DependencyProperty FormDefinitionProperty = DependencyProperty.Register(
            "FormDefinition", typeof(FormDefinition), typeof(NZazuView), new PropertyMetadata(default(FormDefinition), FormDefinitionChanged));

        public FormDefinition FormDefinition
        {
            get => (FormDefinition)GetValue(FormDefinitionProperty);
            set { SetValue(FormDefinitionProperty, value); }
        }

        private static void FormDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var formDefinition = (FormDefinition)e.NewValue;
            view.UpdateFields(formDefinition, view.FieldFactory, view.ResolveLayout);
        }

        // ############# FieldFactory

        public static readonly DependencyProperty FieldFactoryProperty = DependencyProperty.Register(
            "FieldFactory", typeof(INZazuWpfFieldFactory), typeof(NZazuView), new PropertyMetadata(new NZazuFieldFactory(), FieldFactoryChanged, FieldFactoryCoerceCallback));

        public INZazuWpfFieldFactory FieldFactory
        {
            get => (INZazuWpfFieldFactory)GetValue(FieldFactoryProperty);
            set { SetValue(FieldFactoryProperty, value); }
        }

        private static void FieldFactoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var fieldFactory = (INZazuWpfFieldFactory)e.NewValue;
            fieldFactory.View = view;
            view.UpdateFields(view.FormDefinition, fieldFactory, view.ResolveLayout);
        }

        private static object FieldFactoryCoerceCallback(DependencyObject d, object basevalue)
        {
            var view = (NZazuView)d;
            var fieldFactory = (INZazuWpfFieldFactory)basevalue;
            return fieldFactory ?? view.FieldFactory;
        }

        // ############# ResolveLayout

        public static readonly DependencyProperty ResolveLayoutProperty = DependencyProperty.Register(
            "ResolveLayout", typeof(IResolveLayout), typeof(NZazuView),
            new PropertyMetadata(new ResolveLayout(), ResolveLayoutChanged, ResolveLayoutCoerceCallback));

        public IResolveLayout ResolveLayout
        {
            get => (IResolveLayout)GetValue(ResolveLayoutProperty);
            set { SetValue(ResolveLayoutProperty, value); }
        }

        private static void ResolveLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var layoutStrategy = (IResolveLayout)e.NewValue;
            view.UpdateFields(view.FormDefinition, view.FieldFactory, layoutStrategy);
        }

        private static object ResolveLayoutCoerceCallback(DependencyObject d, object basevalue)
        {
            var view = (NZazuView)d;
            var layout = (IResolveLayout)basevalue;
            return layout ?? view.ResolveLayout;
        }

        // ############# FormData

        public static readonly DependencyProperty FormDataProperty = DependencyProperty.Register(
            "FormData", typeof(FormData), typeof(NZazuView),
            new PropertyMetadata(new FormData(), FormDataChanged));

        private static void FormDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (INZazuWpfView)d;
            var fieldValues = (FormData)e.NewValue;
            view.SetFieldValues(fieldValues.Values);

            //if (((Control)view).IsKeyboardFocusWithin == false)
            //    view.TrySetFocusOn();

            //if (e.OldValue != e.NewValue &&fieldValues.Values.ContainsKey("__focusOn"))
            //    view.TrySetFocusOn(fieldValues.Values["__focusOn"]);
        }


        public FormData FormData
        {
            get => (FormData)GetValue(FormDataProperty);
            set { SetValue(FormDataProperty, value); }
        }


        // ############# IsReadOnly

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
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion

        public NZazuView()
        {
            InitializeComponent();
        }

        private ContentControl Layout => this;

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
                // remember ctrl with focus for state
                var focusIsAt = GetFocussedControl(e.NewFocus as FrameworkElement);

                _lastFoussedElement = focusIsAt;
                // now if I focus on the control, I focus on the last field
                if (Equals(e.NewFocus, Layout))
                    TrySetFocusOn();
            };

            FieldFactory = new NZazuFieldFactory(new NZazuFieldBehaviorFactory(), new CheckFactory(), new NZazuXmlSerializer());
        }

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
                .Where(f => !string.IsNullOrEmpty(f.Value.StringValue))
                .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.StringValue))
                // now lets add the control state
                .Concat(_fields
                    .Where(f => !string.IsNullOrEmpty(f.Value.StringValue))
                    .SelectMany(x => x.Value.GetState()))
                // and dont forget the focus
                .Concat(new[] { new KeyValuePair<string, string>("__focusOn", _lastFoussedElement?.Key), })
                .ToDictionary(x => x.Key, x => x.Value);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private INZazuWpfField GetFocussedControl(UIElement focusedSubControl)
        {
            if (focusedSubControl == null) return null;

            var nzfield = _fields.SingleOrDefault(x => Equals(x.Value?.ValueControl, focusedSubControl)).Value;

            if (nzfield != null) return nzfield;

            var parent = VisualTreeHelper.GetParent(focusedSubControl);
            if (parent != null)
                return GetFocussedControl(parent as UIElement);

            return null;
        }

        public ValueCheckResult Validate()
        {
            var result = _fields.Values.Select(f => f.Validate()).FirstOrDefault(vr => !vr.IsValid);
            return result ?? ValueCheckResult.Success;
        }

        public bool TrySetFocusOn(string focusOn = null, bool force = false)
        {
            INZazuWpfField field;
            var safeFocusOn = focusOn ?? (FormData.Values.ContainsKey(FocusOnFieldName) ? FormData.Values[FocusOnFieldName] : null);
            if (string.IsNullOrWhiteSpace(safeFocusOn) || !TryGetField(safeFocusOn, out field)) return false;

            var control = field.ValueControl;
            if (control == null) return false;
            if (force) this.RemoveFocus();

            control.SetFocus();
            control.DelayedFocus();

            return true;
        }

        private readonly IDictionary<string, INZazuWpfField> _fields = new Dictionary<string, INZazuWpfField>();
        private INZazuWpfField _lastFoussedElement;

        private void UpdateFields(
            FormDefinition formDefinition,
            INZazuWpfFieldFactory fieldFactory,
            IResolveLayout resolveLayout)
        {
            DisposeFields();

            // make sure at least the minimum is set for render the layout
            if (formDefinition?.Fields == null) return;

            CreateFields(formDefinition, fieldFactory);

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

        private void CreateFields(FormDefinition formDefinition, INZazuWpfFieldFactory fieldFactory)
        {
            formDefinition.Fields.ToList().ForEach(f =>
            {
                // create field
                var field = fieldFactory.CreateField(f);
                _fields.Add(f.Key, field);
                AddGroupFieldKeys(field as INZazuWpfFieldContainer);
            });
        }

        private void AddGroupFieldKeys(INZazuWpfFieldContainer groupField)
        {
            if (groupField?.Fields == null) return;
            foreach (var field in groupField.Fields)
            {
                _fields.Add(field.Key, field);
                AddGroupFieldKeys(field as INZazuWpfFieldContainer);
            }
        }

        private void DisposeFields()
        {
            foreach (var field in _fields.Values)
                field.DisposeField();

            _fields.Clear();
        }
    }
}