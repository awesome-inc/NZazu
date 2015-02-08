using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    public abstract class NZazuField : INZazuField
    {
        private readonly Lazy<Control> _labelControl;
        private readonly Lazy<Control> _valueControl;

        protected NZazuField(string key)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");
            Key = key;

            _labelControl = new Lazy<Control>(GetLabelControl);
            _valueControl = new Lazy<Control>(GetValueControl);
            Settings = new Dictionary<string, string>();
        }

        public abstract string StringValue { get; set; }
        public abstract DependencyProperty ContentProperty { get; }

        public abstract string Type { get; }
        public string Key { get; private set; }
        public string Prompt { get; protected internal set; }
        public string Hint { get; protected internal set; }
        public string Description { get; protected internal set; }

        public Control LabelControl { get { return _labelControl.Value; } }
        public Control ValueControl { get { return _valueControl.Value; } }
        public Dictionary<string, string> Settings { get; protected internal set; }

        public void Validate()
        {
            var bindingExpression = ContentProperty != null 
                ? ValueControl.GetBindingExpression(ContentProperty) 
                : null;
            if (bindingExpression != null && bindingExpression.HasError) 
                throw new ValidationException("UI has errors. Value could not be converted");

            if (Check == null) return;
            // TODO: how to customize the culture?
            Check.Validate(StringValue, CultureInfo.CurrentUICulture);
        }

        protected internal IValueCheck Check { get; set; } // 'internal' required for testing

        protected virtual Control GetLabel() { return !String.IsNullOrWhiteSpace(Prompt) ? new Label { Content = Prompt } : null; }
        protected abstract Control GetValue();

        private Control GetLabelControl()
        {
            return GetLabel();
        }

        private Control GetValueControl()
        {
            var control = GetValue();
            return DecorateValidation(control);
        }

        protected virtual Control DecorateValidation(Control control)
        {
            if (control == null) return null;
            if (ContentProperty == null) return control; // because no validation if no content!

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
            binding = DecorateBinding(binding);
            control.SetBinding(ContentProperty, binding);

            if (Check == null) return control; // no checks, no validation required. saves performance

            binding.ValidationRules.Clear();
            binding.ValidationRules.Add(new CheckValidationRule(Check) { ValidatesOnTargetUpdated = true });

            return control;
        }

        /// <summary>
        /// binding needs to be changed by subclasses for example if the Nullable-binding should be set.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual Binding DecorateBinding(Binding binding) { return binding; }
    }

    public abstract class NZazuField<T> : NZazuField, INZazuField<T>, INotifyPropertyChanged
    {
        private T _value;

        protected NZazuField(string key)
            : base(key)
        {
        }

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

        public override string StringValue
        {
            get { return GetStringValue(); }
            set { SetStringValue(value); }
        }

        protected abstract void SetStringValue(string value);
        protected abstract string GetStringValue();

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        // ReSharper disable once MemberCanBePrivate.Global
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override Binding DecorateBinding(Binding binding)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) == null) return binding;

            binding.TargetNullValue = string.Empty;
            return binding;
        }

    }
}