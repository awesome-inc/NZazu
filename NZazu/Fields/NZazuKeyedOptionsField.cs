using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuKeyedOptionsField
        : NZazuField<string>
    {
        private static readonly List<NZazuKeyedOptionsField> AvailableFields = new List<NZazuKeyedOptionsField>();
        private readonly bool _isPublisherOnly;
        private readonly Dictionary<Guid, string> _optionsCache = new Dictionary<Guid, string>();

        private readonly string _storeKey;
        private readonly ComboBox _valueControl;
        private string _currentValue = string.Empty;
        private string[] _options;

        public NZazuKeyedOptionsField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            Id = Guid.NewGuid();
            _valueControl = CreateValueControlInternal();
            _storeKey = Definition.Settings.Get("storekey") ?? string.Empty;

            var wasParsable = bool.TryParse(Definition.Settings.Get("publisherOnly"), out var isOnlyASource);
            _isPublisherOnly = wasParsable && isOnlyASource;

            AvailableFields.Add(this);
            ValueAdded += OnValueAdded;

            AvailableFields.ForEach(x =>
            {
                OnValueAdded(this, new ValueChangedEventArgs<string>(x._storeKey, x.Id, null, x._currentValue));
            });
        }

        public string[] Options
        {
            get => _options;
            internal set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Length == 0)
                    throw new ArgumentException("Value cannot be an empty collection.", nameof(value));
                SetOptionValues(value);
            }
        }

        // unique id for each control so we dont need to store a reference for it
        private Guid Id { get; }

        public override DependencyProperty ContentProperty => ComboBox.TextProperty;
        private static event EventHandler<ValueChangedEventArgs<string>> ValueAdded = (sender, e) => { };

        protected internal void SetOptionValues(string[] optionValues)
        {
            if (optionValues == null || _valueControl == null) return;
            _options = optionValues;
            foreach (var option in _options)
                _valueControl.Items.Add(option);
            _valueControl.SelectedItem = _options.FirstOrDefault();
        }

        protected override void Dispose(bool disposing)
        {
            ValueAdded.Invoke(this, new ValueChangedEventArgs<string>(_storeKey, Id, _currentValue, null));

            _valueControl.Items.Clear();
            ValueAdded -= OnValueAdded;
            AvailableFields.Remove(this);

            base.Dispose(disposing);
        }

        private void OnValueAdded(object sender, ValueChangedEventArgs<string> e)
        {
            if (e.StoreKey != _storeKey || _isPublisherOnly)
                return;

            if (_optionsCache.ContainsKey(e.CtrlGuid))
                _optionsCache.Remove(e.CtrlGuid);

            if (!string.IsNullOrWhiteSpace(e.NewValue))
                _optionsCache.Add(e.CtrlGuid, e.NewValue);

            _optionsCache
                .Select(x => x.Value)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList()
                .ForEach(x =>
                {
                    if (_valueControl.Items.Contains(x)) return;
                    _valueControl.Items.Add(x);
                });
            _valueControl.Items.OfType<string>()
                .Where(x => !_optionsCache.ContainsValue(x))
                .ToList()
                .ForEach(x => _valueControl.Items.Remove(x));
        }

        private ComboBox CreateValueControlInternal()
        {
            var control = new ComboBox
            {
                ToolTip = Definition.Description,
                IsEditable = true
            };
            control.LostFocus += StoreAndPublishValue;

            return control;
        }

        protected override Control CreateValueControl()
        {
            return _valueControl;
        }

        private void StoreAndPublishValue(object sender, RoutedEventArgs e)
        {
            if (_currentValue == (_valueControl.Text ?? string.Empty))
                return;

            ValueAdded.Invoke(this,
                new ValueChangedEventArgs<string>(_storeKey, Id, _currentValue, _valueControl.Text));
            _currentValue = _valueControl.Text ?? string.Empty;
        }

        public override void SetValue(string value)
        {
            Value = value;
            StoreAndPublishValue(this, new RoutedEventArgs());
        }

        public override string GetValue()
        {
            return Value;
        }
    }
}