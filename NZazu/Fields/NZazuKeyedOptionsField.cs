using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public sealed class NZazuKeyedOptionsField
        : NZazuField<string>
    {
        private static event EventHandler<ValueChangedEventArgs<string>> ValueAdded = (sender, e) => { };

        private readonly string _storeKey;
        private string _currentValue = string.Empty;
        private readonly ComboBox _valueControl;
        private readonly Dictionary<Guid, string> _optionsCache = new Dictionary<Guid, string>();

        public NZazuKeyedOptionsField(FieldDefinition definition) : base(definition)
        {
            _valueControl = CreateValueControlInternal();
            _storeKey = GetSetting("storekey") ?? string.Empty;
            ValueAdded += OnValueAdded;
            Id = Guid.NewGuid();
        }

        public override void DisposeField()
        {
            ValueAdded.Invoke(this, new ValueChangedEventArgs<string>(_storeKey, Id, _currentValue, null));

            base.DisposeField();
        }

        // unique id for each control so we dont need to store a reference for it
        private Guid Id { get; }

        private void OnValueAdded(object sender, ValueChangedEventArgs<string> e)
        {
            if (e.StoreKey != _storeKey)
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