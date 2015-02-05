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
    abstract class NZazuField<T> : INZazuField<T>, INotifyPropertyChanged
    {
        private readonly Lazy<Control> _labelControl;
        private readonly Lazy<Control> _valueControl;
        private T _value;

        public string Type { get; protected set; }

        protected NZazuField(string key)
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

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("StringValue");
            }
        }

        protected internal DependencyProperty ContentProperty { get; protected set; } // 'internal' required for testing
        protected internal IEnumerable<IValueCheck> Checks { get; set; } // 'internal' required for testing

        public string StringValue
        {
            get { return GetStringValue(); }
            set { SetStringValue(value); }
        }

        protected abstract void SetStringValue(string value);
        protected abstract string GetStringValue();

        public void Validate()
        {
            var safeChecks = Checks == null ? new IValueCheck[] { } : Checks.ToArray();
            new AggregateCheck(safeChecks).Validate(StringValue, CultureInfo.CurrentUICulture);
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

            //if (Nullable.GetUnderlyingType(typeof (T)) != null)
            //    binding.TargetNullValue = default(T);

            control.SetBinding(ContentProperty, binding);

            if (Checks == null || !Checks.Any()) return control; // no checks, no validation required. saves performance

            var safeChecks = Checks == null ? new IValueCheck[] { } : Checks.ToArray();
            var aggregateCheck = new AggregateCheck(safeChecks);
            binding.ValidationRules.Clear();
            binding.ValidationRules.Add(new CheckValidationRule(aggregateCheck) { ValidatesOnTargetUpdated = true });

            return control;
        }

        protected virtual IValueConverter GetCconverter()
        {
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        // ReSharper disable once MemberCanBePrivate.Global
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class NZazuField : NZazuField<string>
    {
        public NZazuField(string key) : base(key)
        {
        }

        protected override void SetStringValue(string value)
        {
            Value = value;
        }

        protected override string GetStringValue()
        {
            return Value;
        }
    }
}