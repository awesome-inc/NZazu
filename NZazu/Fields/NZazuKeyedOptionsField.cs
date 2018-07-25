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
        private static event EventHandler<ValueChangedEventArgs<string>> ValueAdded = (sender, e) => { };
        private static readonly List<NZazuKeyedOptionsField> AvailableFields = new List<NZazuKeyedOptionsField>();

        private readonly string _storeKey;
        private readonly bool _isPublisherOnly;
        private string _currentValue = string.Empty;
        private readonly ComboBox _valueControl;
        private readonly Dictionary<Guid, string> _optionsCache = new Dictionary<Guid, string>();
        private string[] _options;

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

        protected internal void SetOptionValues(string[] optionValues)
        {
            if (optionValues == null || _valueControl == null) return;
            _options = optionValues;
            foreach (var option in _options)
                _valueControl.Items.Add(option);
            _valueControl.SelectedItem = _options.FirstOrDefault();
        }

        public NZazuKeyedOptionsField(FieldDefinition definition) : base(definition)
        {
            Id = Guid.NewGuid();
            _valueControl = CreateValueControlInternal();
            _storeKey = GetSetting("storekey") ?? string.Empty;

            var wasParsable = bool.TryParse(GetSetting("publisherOnly"), out var isOnlyASource);
            _isPublisherOnly = wasParsable && isOnlyASource;

            AvailableFields.Add(this);
            ValueAdded += OnValueAdded;

            AvailableFields.ForEach(x =>
            {
                OnValueAdded(this, new ValueChangedEventArgs<string>(x._storeKey, x.Id, null, x._currentValue));
            });
        }

        public override void DisposeField()
        {
            ValueAdded.Invoke(this, new ValueChangedEventArgs<string>(_storeKey, Id, _currentValue, null));
            AvailableFields.Remove(this);

            base.DisposeField();
        }

        // unique id for each control so we dont need to store a reference for it
        private Guid Id { get; }

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

        public override DependencyProperty ContentProperty => ComboBox.TextProperty;

        public override string Type => "keyedoption";

        private ComboBox CreateValueControlInternal()
        {
            var control = new ComboBox
            {
                ToolTip = Description,
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

            ValueAdded.Invoke(this, new ValueChangedEventArgs<string>(_storeKey, Id, _currentValue, _valueControl.Text));
            _currentValue = _valueControl.Text ?? string.Empty;
        }

        protected override void SetStringValue(string value)
        {
            Value = value;
            StoreAndPublishValue(this, new RoutedEventArgs());
        }
        protected override string GetStringValue() { return Value; }
    }
}