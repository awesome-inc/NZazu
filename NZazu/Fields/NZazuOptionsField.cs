using System;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuOptionsField : NZazuField<string>
    {
        private string[] _options;

        public NZazuOptionsField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
        }

        public override DependencyProperty ContentProperty => ComboBox.TextProperty;


        public string[] Options
        {
            get => _options;
            protected internal set
            {
                _options = value;
                UpdateControlValues((ComboBox) ValueControl);
            }
        }

        private void UpdateControlValues(ComboBox control)
        {
            var currentValue = GetValue();
            control.Items.Clear();
            if (Options == null) return;

            foreach (var option in Options)
                control.Items.Add(option);
            control.SelectedItem = currentValue; // Options.FirstOrDefault();
        }

        protected override Control CreateValueControl()
        {
            var control = new ComboBox {ToolTip = Definition.Description};
            UpdateControlValues(control);
            return control;
        }

        public override void SetValue(string value)
        {
            Value = value;
        }

        public override string GetValue()
        {
            return Value;
        }
    }
}