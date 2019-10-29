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
    public abstract class NZazuField
        : INZazuWpfField
        , INotifyPropertyChanged
    {
        protected NZazuField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            if (serviceLocatorFunc == null) throw new ArgumentNullException(nameof(serviceLocatorFunc));

            _labelControl = new Lazy<Control>(CreateLabelControl);
            _valueControl = new Lazy<Control>(() =>
            {
                var ctrl = CreateValueControl();
                if (ctrl == null) return null;

                ctrl.LostFocus += (sender, e) => { Validate(); };
                AddValuePropertyBinding(this, ctrl);

                return ctrl;
            });
            FormatProvider = serviceLocatorFunc(typeof(IFormatProvider)) as IFormatProvider
                             ?? throw new ArgumentNullException(nameof(FormatProvider),
                                 "no IFormatProvider implementation in ServiceLocator from factory");
            ValueConverter = serviceLocatorFunc(typeof(IValueConverter)) as IValueConverter
                             ?? throw new ArgumentNullException(nameof(ValueConverter),
                                 "no IValueConverter implementation in ServiceLocator from factory");


            if (string.IsNullOrWhiteSpace(definition.Key)) throw new ArgumentException("key");
            Key = definition.Key;
        }

        protected internal IEnumerable<INZazuWpfFieldBehavior> Behaviors { get; set; } =
            new List<INZazuWpfFieldBehavior>();

        protected internal IValueCheck Check
        {
            get => _check;
            set
            {
                _check = value;
                UpdateBindingValidation(_check, _binding);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public bool IsEditable { get; protected set; } = true;
        public string Key { get; }

        public FieldDefinition Definition { get; }

        public abstract ValueCheckResult Validate();

        public virtual IEnumerable<KeyValuePair<string, string>> GetState()
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }


        /// <summary>
        ///     binding needs to be changed by subclasses for example if the Nullable-binding should be set.
        /// </summary>
        /// <param name="binding"></param>
        protected internal virtual Binding DecorateBinding(Binding binding)
        {
            return binding;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // this adds the binding. needs to be done here for control testing
        // otherwise values are not added to the value control
        private static void AddValuePropertyBinding(NZazuField field, Control control)
        {
            if (control == null) return;
            if (field.ContentProperty == null) return;

            _binding = new Binding("Value")
            {
                Source = field,
                Mode = BindingMode.TwoWay,
                ValidatesOnDataErrors = true,
                ValidatesOnExceptions = true,
                NotifyOnValidationError = true,
                NotifyOnTargetUpdated = true,
                NotifyOnSourceUpdated = true,
                IsAsync = false,
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            if (field.ValueConverter != null)
                _binding.Converter = field.ValueConverter;

            _binding = field.DecorateBinding(_binding);

            UpdateBindingValidation(field.Check, _binding);

            control.SetBinding(field.ContentProperty, _binding);
        }

        private static void UpdateBindingValidation(IValueCheck check, Binding binding)
        {
            binding?.ValidationRules.Clear();

            if (check == null || binding == null) return;
            binding.ValidationRules.Add(new CheckValidationRule(check) { ValidatesOnTargetUpdated = true });
        }

        #region private fields and wrapper

        // lazy controls which should be loaded on first access
        private readonly Lazy<Control> _labelControl;
        private readonly Lazy<Control> _valueControl;
        public Control LabelControl => _labelControl.Value;
        public Control ValueControl => _valueControl.Value;

        protected readonly IFormatProvider FormatProvider;
        protected internal IValueConverter ValueConverter;
        private IValueCheck _check;
        private static Binding _binding;

        #endregion

        #region abstract methods

        public abstract void SetValue(string value);
        public abstract string GetValue();
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            Behaviors?.ToList().ForEach(x => { x?.Detach(); });
            Behaviors = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public abstract class NZazuField<T> : NZazuField, INZazuWpfField<T>
    {
        private T _value;

        protected NZazuField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
        }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public override ValueCheckResult Validate()
        {
            var bindingExpression = ContentProperty != null
                ? ValueControl.GetBindingExpression(ContentProperty)
                : null;
            if (bindingExpression != null && bindingExpression.HasError)
                return new ValueCheckResult(new Exception($"{Definition.Prompt}: {bindingExpression.ValidationError?.ErrorContent as string ?? "UI has errors. Value could not be converted"}"));

            if (Check == null) return ValueCheckResult.Success;

            var result = Check.Validate(GetValue(), Value, FormatProvider);

            return result;
        }

        protected internal override Binding DecorateBinding(Binding binding)
        {
            return binding;
        }
    }
}