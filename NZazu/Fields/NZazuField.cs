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
    public abstract class NZazuField : INZazuWpfField
    {
        protected IFormatProvider FormatProvider { get { return CultureInfo.InvariantCulture; } }

        private readonly Lazy<Control> _labelControl;
        private readonly Lazy<Control> _valueControl;

        public abstract bool IsEditable { get; }
        public abstract string StringValue { get; set; }
        public abstract DependencyProperty ContentProperty { get; }

        public abstract string Type { get; }
        public string Key { get; private set; }
        public string Prompt { get; protected internal set; }
        public string Hint { get; protected internal set; }
        public string Description { get; protected internal set; }

        public Control LabelControl { get { return _labelControl.Value; } }
        public Control ValueControl { get { return _valueControl.Value; } }
        public Dictionary<string, string> Settings { get; private set; }
        public INZazuWpfFieldBehavior Behavior { get; set; }

        protected NZazuField(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");
            Key = key;

            _labelControl = new Lazy<Control>(GetLabelControl);
            _valueControl = new Lazy<Control>(GetValueControl);
            Settings = new Dictionary<string, string>();
        }

        public ValueCheckResult Validate()
        {
            var bindingExpression = ContentProperty != null 
                ? ValueControl.GetBindingExpression(ContentProperty) 
                : null;
            if (bindingExpression != null && bindingExpression.HasError) 
                return new ValueCheckResult(false, "UI has errors. Value could not be converted");

            if (Check == null) return ValueCheckResult.Success;

            // TODO: how to customize the culture?
            return Check.Validate(StringValue, FormatProvider);
        }

        protected internal IValueCheck Check { get; set; }

        protected virtual Control GetLabel() { return !string.IsNullOrWhiteSpace(Prompt) ? new Label { Content = Prompt } : null; }
        protected abstract Control GetValue();

        private Control GetLabelControl()
        {
            return GetLabel();
        }

        private Control GetValueControl()
        {
            var control = GetValue();
            ApplySettings(control);
            return DecorateValidation(control);
        }

        private void ApplySettings(Control control)
        {
            var height = GetSetting<double>("Height");
            if (height.HasValue)
                control.MinHeight = control.MaxHeight = height.Value;
            var width = GetSetting<double>("Width");
            if (width.HasValue)
                control.MinWidth = control.MaxWidth = width.Value;

            ApplyGenericSettings(control);
        }

        private void ApplyGenericSettings(Control control)
        {
            var controlSettings = Settings.Where(settings => control.CanSetProperty(settings.Key));
            foreach (var setting in controlSettings)
                control.SetProperty(setting.Key, setting.Value);
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

        protected string GetSetting(string key)
        {
            string value;
            Settings.TryGetValue(key, out value);
            return value;
        }

        protected internal T? GetSetting<T>(string key) where T:struct
        {
            try
            {
                var str = GetSetting(key);
                return (T)Convert.ChangeType(str, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception) { return null; }
        }
    }

    public abstract class NZazuField<T> : NZazuField, INZazuWpfField<T>, INotifyPropertyChanged
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

        public override bool IsEditable { get { return true; } }

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