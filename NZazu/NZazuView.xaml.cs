using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NZazu.Contracts;

namespace NZazu
{
    public partial class NZazuView : INZazuView
    {
        public static readonly DependencyProperty FormDefinitionProperty = DependencyProperty.Register(
            "FormDefinition", typeof(FormDefinition), typeof(NZazuView), new PropertyMetadata(default(FormDefinition), FormDefinitionChanged));

        public static readonly DependencyProperty FieldFactoryProperty = DependencyProperty.Register(
            "FieldFactory", typeof(INZazuFieldFactory), typeof(NZazuView), new PropertyMetadata(new NZazuFieldFactory()));

        public static readonly DependencyProperty LayoutStrategyProperty = DependencyProperty.Register(
            "LayoutStrategy", typeof(INZazuLayoutStrategy), typeof(NZazuView), new PropertyMetadata(new GridLayoutStrategy()));

        public INZazuLayoutStrategy LayoutStrategy
        {
            get { return (INZazuLayoutStrategy)GetValue(LayoutStrategyProperty); }
            set { SetValue(LayoutStrategyProperty, value); }
        }

        public INZazuFieldFactory FieldFactory
        {
            get { return (INZazuFieldFactory)GetValue(FieldFactoryProperty); }
            set { SetValue(FieldFactoryProperty, value); }
        }

        private readonly IDictionary<string, INZazuField> _fields = new Dictionary<string, INZazuField>();

        private static void FormDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (NZazuView)d;
            var formDefinition = (FormDefinition)e.NewValue;
            view.UpdateFields(formDefinition);
        }

        private void UpdateFields(FormDefinition formDefinition)
        {
            _fields.Clear();
            // todo clear item in content control

            formDefinition.Fields.ToList().ForEach(f => _fields.Add(f.Key, FieldFactory.CreateField(f)));
            LayoutStrategy.DoLayout(Layout, _fields.Values);
        }

        public FormDefinition FormDefinition
        {
            get { return (FormDefinition)GetValue(FormDefinitionProperty); }
            set { SetValue(FormDefinitionProperty, value); }
        }

        public INZazuField GetField(string fieldKey)
        {
            return _fields[fieldKey];
        }

        public NZazuView()
        {
            InitializeComponent();
        }
    }
}
