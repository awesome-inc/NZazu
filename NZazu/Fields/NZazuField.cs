using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    class NZazuField : INZazuField, INotifyPropertyChanged
    {
        private readonly Lazy<Control> _labelControl;
        private readonly Lazy<Control> _valueControl;
        private string _value;

        public string Type { get; protected set; }
        public NZazuField(string key)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");
            Key = key;
            Type = "label";

            _labelControl = new Lazy<Control>(GetLabelControl);
            _valueControl = new Lazy<Control>(GetValueControl);
        }

        public string Key { get; private set; }
        public string Prompt { get; protected internal set; }
        public string Hint { get; protected internal set; }
        public string Description { get; protected internal set; }

        protected internal DependencyProperty ContentProperty { get; protected set; } // 'internal' required for testing
        protected internal IEnumerable<IValueCheck> Checks { get; set; } // 'internal' required for testing

        // todo: make this more generic so it does not need to be virtual
        public virtual string Value
        {
            get
            {
                // todo ContentProperty.PropertyType != typeof(string) then... ConvertToString
                return _value;
            }
            set
            {
                // todo ContentProperty.PropertyType != typeof(string) then... ConvertFromString
                if (_value == value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public void Validate()
        {
            var safeChecks = Checks == null ? new IValueCheck[] { } : Checks.ToArray();
            new AggregateCheck(safeChecks).Validate(_value, CultureInfo.CurrentUICulture);
        }

        public Control LabelControl { get { return _labelControl.Value; } }
        public Control ValueControl { get { return _valueControl.Value; } }

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

            if (control.GetBindingExpression(ContentProperty) != null) throw new InvalidOperationException("binding already applied.");
            var binding = new Binding("Value")
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                ValidatesOnDataErrors = true,
                ValidatesOnExceptions = true,
                NotifyOnValidationError = true,
                NotifyOnTargetUpdated = true,
                NotifyOnSourceUpdated = true,
                IsAsync = false,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            control.SetBinding(ContentProperty, binding);

            if (Checks == null || !Checks.Any()) return control; // no checks, no validation required. saves performance

            var safeChecks = Checks == null ? new IValueCheck[] { } : Checks.ToArray();
            var aggregateCheck = new AggregateCheck(safeChecks);
            binding.ValidationRules.Clear();
            binding.ValidationRules.Add(new CheckValidationRule(aggregateCheck) { ValidatesOnTargetUpdated = true });

            return control;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        // ReSharper disable once MemberCanBePrivate.Global
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}