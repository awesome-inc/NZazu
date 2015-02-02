using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NZazu.Contracts;
using NZazu.Layout;

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

        public NZazuView()
        {
            InitializeComponent();
        }

        public FormDefinition FormDefinition
        {
            get { return (FormDefinition)GetValue(FormDefinitionProperty); }
            set { SetValue(FormDefinitionProperty, value); }
        }

        public INZazuFieldFactory FieldFactory
        {
            get { return (INZazuFieldFactory)GetValue(FieldFactoryProperty); }
            set { SetValue(FieldFactoryProperty, value); }
        }

        public INZazuLayoutStrategy LayoutStrategy
        {
            get { return (INZazuLayoutStrategy)GetValue(LayoutStrategyProperty); }
            set { SetValue(LayoutStrategyProperty, value); }
        }

        public INZazuField GetField(string fieldKey)
        {
            return _fields[fieldKey];
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
            formDefinition.Fields.ToList().ForEach(f => _fields.Add(f.Key, FieldFactory.CreateField(f)));
            LayoutStrategy.DoLayout(Layout, _fields.Values);
        }
    }
}
