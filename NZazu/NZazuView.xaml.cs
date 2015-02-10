﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NZazu.Contracts;
using NZazu.Extensions;
using NZazu.Fields;

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
            view.UpdateFields(formDefinition, view.FieldFactory, view.ResolveLayout);
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
            get { return (IResolveLayout)GetValue(ResolveLayoutProperty); }
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

        public INZazuWpfField GetField(string key)
        {
            INZazuWpfField field;
            if (!_fields.TryGetValue(key, out field)
                && !_groupFields.TryGetValue(key, out field))
                throw new KeyNotFoundException();
            return field;
        }

        public void ApplyChanges()
        {
            FormData = GetFieldValues();
        }

        public Dictionary<string, string> GetFieldValues()
        {
            return (_fields.Concat(_groupFields)).ToDictionary(f => f.Key, f => f.Value.StringValue);
        }


        public void Validate()
        {
            _fields.Values.ToList().ForEach(f => f.Validate());
        }

        private readonly IDictionary<string, INZazuWpfField> _fields = new Dictionary<string, INZazuWpfField>();
        private readonly IDictionary<string, INZazuWpfField> _groupFields = new Dictionary<string, INZazuWpfField>();

        private void UpdateFields(FormDefinition formDefinition, INZazuWpfFieldFactory fieldFactory, IResolveLayout resolveLayout)
        {
            DisposeFields();

            // make sure at least the minimum is set for render the layout
            if (formDefinition == null) return;
            if (formDefinition.Fields == null) return;

            CreateFields(formDefinition, fieldFactory);
            AttachBehavior();

            var layout = resolveLayout.Resolve(formDefinition.Layout);
            layout.DoLayout(Layout, _fields.Values, resolveLayout);
        }

        private void CreateFields(FormDefinition formDefinition, INZazuWpfFieldFactory fieldFactory)
        {
            formDefinition.Fields.ToList().ForEach(f =>
            {
                var field = fieldFactory.CreateField(f);
                _fields.Add(f.Key, field);
                AddGroupFieldKeys(field as INZazuWpfGroupField);
            });
        }

        private void AddGroupFieldKeys(INZazuWpfGroupField groupField)
        {
            if (groupField == null) return;
            foreach (var field in groupField.Fields)
            {
                _groupFields.Add(field.Key, field);
                AddGroupFieldKeys(field as INZazuWpfGroupField);
            }
        }

        private void DisposeFields()
        {
            DetachBehavior();
            _fields.Clear();
            _groupFields.Clear();
        }

        private void AttachBehavior()
        {
        }

        private void DetachBehavior()
        {
        }
    }
}
