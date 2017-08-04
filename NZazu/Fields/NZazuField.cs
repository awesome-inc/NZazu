using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Extensions;

namespace NZazu.Fields
{
    public abstract class NZazuField : INZazuWpfField
    {
        protected readonly FieldDefinition Definition;
        protected IFormatProvider FormatProvider => CultureInfo.InvariantCulture;

        private readonly Lazy<Control> _labelControl;
        private readonly Lazy<Control> _valueControl;
        private readonly object _lockObj = new object();
        protected Control Control;

        public abstract bool IsEditable { get; }
        public string StringValue
        {
            get { return GetStringValue(); }
            set { SetStringValue(value); }
        }

        protected abstract void SetStringValue(string value);
        protected abstract string GetStringValue();
        public abstract DependencyProperty ContentProperty { get; }

        public abstract string Type { get; }
        public string Key { get; }
        public string Prompt { get; protected internal set; }
        public string Hint { get; protected internal set; }
        public string Description { get; protected internal set; }

        public Control LabelControl => _labelControl.Value;
        public Control ValueControl => _valueControl.Value;
        public Dictionary<string, string> Settings { get; }
        public INZazuWpfFieldBehavior Behavior { get; set; }

        protected NZazuField(FieldDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (string.IsNullOrWhiteSpace(definition.Key)) throw new ArgumentException("key");
            Definition = definition;
            Key = definition.Key;
            Description = definition.Description;

            _labelControl = new Lazy<Control>(GetLabelControl);
            _valueControl = new Lazy<Control>(GetValueControl);
            Settings = new Dictionary<string, string>();
        }

        public virtual ValueCheckResult Validate()
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

        protected virtual Control CreateLabelControl() { return !string.IsNullOrWhiteSpace(Prompt) ? new Label { Content = Prompt } : null; }
        protected abstract Control CreateValueControl();

        private Control GetLabelControl()
        {
            return CreateLabelControl();
        }

        private Control GetValueControl()
        {
            if (Control != null) return Control;

            lock (_lockObj)
            {
                Control = CreateValueControl();
                ApplySettings(Control);
                DecorateValidation(Control);
            }

            return Control;
        }

        private Control ApplySettings(Control control)
        {
            var height = GetSetting<double>("Height");
            if (height.HasValue)
                control.MinHeight = control.MaxHeight = height.Value;
            var width = GetSetting<double>("Width");
            if (width.HasValue)
                control.MinWidth = control.MaxWidth = width.Value;

            ApplyGenericSettings(control);
            return control;
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
                NotifyOnValidationError = false,
                NotifyOnTargetUpdated = true,
                NotifyOnSourceUpdated = true,
                IsAsync = false,
                Converter = new DebugConverter(), // awesome stuff to debug 
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            binding.TargetNullValue = string.Empty;
            binding = DecorateBinding(binding);

            if (Check == null)
            {
                control.SetBinding(ContentProperty, binding);
                return control; // no checks, no validation required. saves performance
            }

            binding.ValidationRules.Clear();
            binding.ValidationRules.Add(new CheckValidationRule(Check) { ValidatesOnTargetUpdated = true });

            control.SetBinding(ContentProperty, binding);
            return control;
        }

        /// <summary>
        /// binding needs to be changed by subclasses for example if the Nullable-binding should be set.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual Binding DecorateBinding(Binding binding) { return binding; }

        protected virtual string GetSetting(string key)
        {
            string value;
            Settings.TryGetValue(key, out value);
            return value;
        }

        protected internal virtual T? GetSetting<T>(string key) where T : struct
        {
            try
            {
                var str = GetSetting(key);
                return (T)Convert.ChangeType(str, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception) { return null; }
        }

        public virtual void DisposeField()
        {
            if (Behavior == null) return;
            Behavior.Detach();
            Behavior = null;
        }

        public virtual IEnumerable<KeyValuePair<string, string>> GetState()
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }
    }

    public abstract class NZazuField<T> : NZazuField, INZazuWpfField<T>, INotifyPropertyChanged
    {
        private T _value;

        protected NZazuField(FieldDefinition definition) : base(definition) { }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged();
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("StringValue");
            }
        }

        public override bool IsEditable => true;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        // ReSharper disable once MemberCanBePrivate.Global
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override Binding DecorateBinding(Binding binding)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) == null) return binding;

            return binding;
        }
    }
}