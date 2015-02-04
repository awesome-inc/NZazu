using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    class NZazuField : INZazuField
    {
        private readonly Lazy<Control> _label;
        private readonly Lazy<Control> _value;

        public string Type { get; protected set; }
        public NZazuField(string key)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");
            Key = key;
            Type = "label";
            //ContentProperty is null by default 
            //ContentProperty = ContentControl.ContentProperty;

            _label = new Lazy<Control>(GetLabelControl);
            _value = new Lazy<Control>(GetValueControl);
        }

        public string Key { get; private set; }
        public string Prompt { get; protected internal set; }
        public string Hint { get; protected internal set; }
        public string Description { get; protected internal set; }

        protected internal DependencyProperty ContentProperty { get; set; } // 'internal' required for testing
        protected internal IEnumerable<IValueCheck> Checks { get; set; } // 'internal' required for testing

        // todo: make this more generic so it does not need to be virtual
        public virtual string Value
        {
            get
            {
                // todo ContentProperty.PropertyType != typeof(string) then... ConvertToString
                if (ContentProperty == null) return String.Empty;
                return (string)ValueControl.GetValue(ContentProperty);
            }
            set
            {
                // todo ContentProperty.PropertyType != typeof(string) then... ConvertFromString
                if (ContentProperty == null) return;
                ValueControl.SetValue(ContentProperty, value); // TODO issues with non-string fields like numeric
            }
        }

        public Control LabelControl { get { return _label.Value; } }
        public Control ValueControl { get { return _value.Value; } }

        protected virtual Control GetLabel() { return !String.IsNullOrWhiteSpace(Prompt) ? new Label { Content = Prompt } : null; }
        protected virtual Control GetValue() { return !String.IsNullOrWhiteSpace(Description) ? new Label { Content = Description } : null; }

        private Control GetLabelControl()
        {
            return GetLabel();
        }

        private Control GetValueControl()
        {
            var control = GetValue();
            return DecorateValidation(control);
        }

        private Control DecorateValidation(Control control)
        {
            if (control == null) return null;
            if (ContentProperty == null) return control; // because no validation if no content!
            if (Checks == null || !Checks.Any()) return control; // no checks, no validation required. saves performance

            if (control.GetBindingExpression(ContentProperty) != null) throw new InvalidOperationException("binding already applied.");
            var binding = new Binding(ContentProperty.Name)
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                Mode = BindingMode.Default,
                ValidatesOnDataErrors = true,
                ValidatesOnExceptions = true,
                NotifyOnValidationError = true,
                NotifyOnTargetUpdated = true,
                NotifyOnSourceUpdated = true,
                IsAsync = false,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            control.SetBinding(ContentProperty, binding);

            var safeChecks = Checks == null ? new IValueCheck[] { } : Checks.ToArray();
            binding.ValidationRules.Clear();
            binding.ValidationRules.Add(new CheckValidationRule(new AggregateCheck(safeChecks)));

            return control;
        }
    }
}