using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NZazu.Contracts;
using NZazu.Extensions;
using NZazu.Fields;
using NZazu.LayoutStrategy;

namespace NZazu
{
    public partial class NZazuView : INZazuWpfView
    {
        #region dependency properties

        // ############# FormDefinition

        public static readonly DependencyProperty FormDefinitionProperty = DependencyProperty.Register(
            "FormDefinition", typeof(FormDefinition), typeof(NZazuView), new PropertyMetadata(default(FormDefinition), FormDefinitionChanged));

        public FormDefinition FormDefinition
        {
            get { return (FormDefinition)GetValue(FormDefinitionProperty); }
            set { SetValue(FormDefinitionProperty, value); }
        }

        private static void FormDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var formDefinition = (FormDefinition)e.NewValue;
            view.UpdateFields(formDefinition, view.FieldFactory, view.LayoutStrategy);
        }

        // ############# FieldFactory

        public static readonly DependencyProperty FieldFactoryProperty = DependencyProperty.Register(
           "FieldFactory", typeof(INZazuWpfFieldFactory), typeof(NZazuView), new PropertyMetadata(new NZazuFieldFactory(), FieldFactoryChanged, FieldFactoryCoerceCallback));

        public INZazuWpfFieldFactory FieldFactory
        {
            get { return (INZazuWpfFieldFactory)GetValue(FieldFactoryProperty); }
            set { SetValue(FieldFactoryProperty, value); }
        }

        private static void FieldFactoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var fieldFactory = (INZazuWpfFieldFactory)e.NewValue;
            view.UpdateFields(view.FormDefinition, fieldFactory, view.LayoutStrategy);
        }

        private static object FieldFactoryCoerceCallback(DependencyObject d, object basevalue)
        {
            var view = (NZazuView)d;
            var fieldFactory = (INZazuWpfFieldFactory)basevalue;
            return fieldFactory ?? view.FieldFactory;
        }

        // ############# LayoutStrategy

        public static readonly DependencyProperty LayoutStrategyProperty = DependencyProperty.Register(
           "LayoutStrategy", typeof(INZazuWpfLayoutStrategy), typeof(NZazuView), new PropertyMetadata(new GridLayoutStrategy(), LayoutStrategyChanged, LayoutStrategyCoerceCallback));

        public INZazuWpfLayoutStrategy LayoutStrategy
        {
            get { return (INZazuWpfLayoutStrategy)GetValue(LayoutStrategyProperty); }
            set { SetValue(LayoutStrategyProperty, value); }
        }

        private static void LayoutStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var layoutStrategy = (INZazuWpfLayoutStrategy)e.NewValue;
            view.UpdateFields(view.FormDefinition, view.FieldFactory, layoutStrategy);
        }

        private static object LayoutStrategyCoerceCallback(DependencyObject d, object basevalue)
        {
            var view = (NZazuView)d;
            var layoutStrategy = (INZazuWpfLayoutStrategy)basevalue;
            return layoutStrategy ?? view.LayoutStrategy;
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
        }

        public FormData FormData
        {
            get { return (FormData)GetValue(FormDataProperty); }
            set { SetValue(FormDataProperty, value); }
        }

        #endregion

        public NZazuView()
        {
            InitializeComponent();
        }

        public INZazuWpfField GetField(string fieldKey)
        {
            return _fields[fieldKey];
        }

        public void ApplyChanges()
        {
            FormData = new FormData(this.GetFieldValues());
        }

        public void Validate()
        {
            _fields.Values.ToList().ForEach(f => f.Validate());
        }

        private readonly IDictionary<string, INZazuWpfField> _fields = new Dictionary<string, INZazuWpfField>();

        private void UpdateFields(FormDefinition formDefinition, INZazuWpfFieldFactory fieldFactory, INZazuWpfLayoutStrategy layoutStrategy)
        {
            DisposeFields();

            // make sure at least the minimum is set for render the layout
            if (formDefinition == null) return;
            if (formDefinition.Fields == null) return;

            CreateFields(formDefinition, fieldFactory);
            AttachBehavior();
            
            layoutStrategy.DoLayout(Layout, _fields.Values);
        }

        private void CreateFields(FormDefinition formDefinition, INZazuWpfFieldFactory fieldFactory)
        {
            formDefinition.Fields.ToList().ForEach(f =>
            {
                var field = fieldFactory.CreateField(f);
                _fields.Add(f.Key, field);
            });
        }

        private void DisposeFields()
        {
            DetachBehavior();
            _fields.Clear();
        }

        private void AttachBehavior()
        {
        }

        private void DetachBehavior()
        {
        }
    }
}
