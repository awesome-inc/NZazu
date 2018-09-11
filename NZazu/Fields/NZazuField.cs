using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    public abstract class NZazuField : INZazuWpfField
    {
        public bool IsEditable { get; protected set; } = true;
        public string Key { get; }

        public FieldDefinition Definition { get; }
        protected internal IEnumerable<INZazuWpfFieldBehavior> Behaviors { get; set; } = new List<INZazuWpfFieldBehavior>();
        protected internal IValueCheck Check { get; set; }

        #region private fields and wrapper

        // lazy controls which should be loaded on first access
        private readonly Lazy<Control> _labelControl;
        private readonly Lazy<Control> _valueControl;
        public Control LabelControl => _labelControl.Value;
        public Control ValueControl => _valueControl.Value;

        protected readonly IFormatProvider FormatProvider;
        protected readonly IValueConverter ValueConverter;

        #endregion

        #region abstract methods

        public abstract void SetStringValue(string value);
        public abstract string GetStringValue();
        public abstract DependencyProperty ContentProperty { get; }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual Control CreateLabelControl()
        {
            return !string.IsNullOrWhiteSpace(Definition.Prompt)
                ? new Label { Content = Definition.Prompt }
                : null;
        }
        protected abstract Control CreateValueControl();

        #endregion

        protected NZazuField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            if (serviceLocatorFunc == null) throw new ArgumentNullException(nameof(serviceLocatorFunc));

            _labelControl = new Lazy<Control>(CreateLabelControl);
            _valueControl = new Lazy<Control>(CreateValueControl);
            FormatProvider = serviceLocatorFunc(typeof(IFormatProvider)) as IFormatProvider
                              ?? throw new ArgumentNullException(nameof(FormatProvider), "no IFormatProvider implementation in ServiceLocator from factory");
            ValueConverter = serviceLocatorFunc(typeof(IValueConverter)) as IValueConverter
                              ?? throw new ArgumentNullException(nameof(ValueConverter), "no IValueConverter implementation in ServiceLocator from factory");


            if (string.IsNullOrWhiteSpace(definition.Key)) throw new ArgumentException("key");
            Key = definition.Key;
        }

        public virtual ValueCheckResult Validate()
        {
            var bindingExpression = ContentProperty != null
                ? ValueControl.GetBindingExpression(ContentProperty)
                : null;
            if (bindingExpression != null && bindingExpression.HasError)
                return new ValueCheckResult(false, "UI has errors. Value could not be converted");

            if (Check == null) return ValueCheckResult.Success;

            return Check.Validate(GetStringValue(), FormatProvider);
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
        protected virtual Binding DecorateBinding(Binding binding) { return binding; }

        public virtual IEnumerable<KeyValuePair<string, string>> GetState()
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public virtual void Dispose()
        {
            Behaviors?.ToList().ForEach(x => { x?.Detach(); });
            Behaviors = null;
        }
    }

    public abstract class NZazuField<T> : NZazuField, INZazuWpfField<T>, INotifyPropertyChanged
    {
        private T _value;

        protected NZazuField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc) { }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

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