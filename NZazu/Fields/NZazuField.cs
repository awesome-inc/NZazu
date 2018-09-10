using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Extensions;

namespace NZazu.Fields
{
    public abstract class NZazuField : INZazuWpfField
    {
        protected readonly FieldDefinition Definition;
        protected IFormatProvider FormatProvider => CultureInfo.InvariantCulture;

        /// <summary>
        /// override this in case you need some information from the factory like serializer or the factory itself
        /// </summary>
        /// <param name="propertyLookup"></param>
        public virtual NZazuField Initialize(Func<Type, object> propertyLookup) => this;

        private readonly Lazy<Control> _labelControl;
        private readonly Lazy<Control> _valueControl;
        private readonly object _lockObj = new object();

        public abstract bool IsEditable { get; }
        public string StringValue
        {
            get => GetStringValue();
            set => SetStringValue(value);
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
        [Obsolete("Please choose '" + nameof(Behaviors) + "' instead")]
        public INZazuWpfFieldBehavior Behavior { get; set; }
        public List<INZazuWpfFieldBehavior> Behaviors { get; set; } = new List<INZazuWpfFieldBehavior>();

        protected NZazuField(FieldDefinition definition, IValueConverter valueConverter = null)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (string.IsNullOrWhiteSpace(definition.Key)) throw new ArgumentException("key");
            Definition = definition;
            Key = definition.Key;
            Description = definition.Description;

            _labelControl = new Lazy<Control>(CreateLabelControl);
            _valueControl = new Lazy<Control>(() =>
            {
                var ctrl = CreateValueControl();
                ApplySettings(ctrl);
                DecorateValidation(ctrl);
                return ctrl;
            });

            ValueConverter = valueConverter ?? NoExceptionsConverter.Instance;

            Settings = new Dictionary<string, string>();
            foreach (var setting in (definition.Settings ?? new Dictionary<string, string>()))
                Settings.Add(setting.Key, setting.Value);
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

        public IValueConverter ValueConverter { get; private set; }

        // ReSharper disable once UnusedMethodReturnValue.Local
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
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            if (ValueConverter != null)
                binding.Converter = ValueConverter;

            binding = DecorateBinding(binding);

            if (Check != null)
            {
                binding.ValidationRules.Clear();
                binding.ValidationRules.Add(new CheckValidationRule(Check) { ValidatesOnTargetUpdated = true });
            }

            control.SetBinding(ContentProperty, binding);
            return control; // no checks, no validation required. saves performance
        }

        /// <summary>
        /// binding needs to be changed by subclasses for example if the Nullable-binding should be set.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual Binding DecorateBinding(Binding binding) { return binding; }

        protected virtual string GetSetting(string key)
        {
            Settings.TryGetValue(key, out var value);
            return value;
        }

        protected internal virtual T? GetSetting<T>(string key) where T : struct
        {
            var str = GetSetting(key);

            try
            {
                if (str == null) return null;
                return (T)Convert.ChangeType(str, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                Trace.TraceWarning($"Setting {key} with value '{str ?? "<null>"}' has the wrong type. A {typeof(T).Name} is expected.");
                return null;
            }
        }

        public virtual void DisposeField()
        {
#pragma warning disable 618
            if (Behavior != null)
            {
                Behavior.Detach();
                Behavior = null;
            }
#pragma warning restore 618

            Behaviors?.ForEach(b =>
            {
                b?.Detach();
            });
            Behaviors = null;
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
            get => _value;
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