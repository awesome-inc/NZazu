using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuOptionsField : NZazuField<string>
    {
        public NZazuOptionsField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc) { }

        public override DependencyProperty ContentProperty => ComboBox.TextProperty;


        public string[] Options { get; protected internal set; }

        protected override Control CreateValueControl()
        {
            var control = new ComboBox { ToolTip = Definition.Description };
            if (Options != null)
            {
                foreach (var option in Options)
                    control.Items.Add(option);
                control.SelectedItem = Options.FirstOrDefault();
            }
            return control;
        }

        public override void SetValue(string value) { Value = value; }
        public override string GetValue() { return Value; }
    }
}