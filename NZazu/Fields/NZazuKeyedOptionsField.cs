using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public sealed class NZazuKeyedOptionsField : NZazuField<string>
    {
        private static event EventHandler<ValueChangedEventArgs<string>> ValueAdded = (sender, e) => { };

        private readonly string _storeKey;
        private string _currentValue = string.Empty;
        private readonly ComboBox _valueControl;

        public NZazuKeyedOptionsField(FieldDefinition definition) : base(definition)
        {
            _valueControl = CreateValueControlInternal();
            _storeKey = GetSetting("storekey") ?? string.Empty;
            ValueAdded += OnValueAdded;
        }

        private void OnValueAdded(object sender, ValueChangedEventArgs<string> e)
        {
            if (e.StoreKey != _storeKey)
                return;

            _valueControl.Items.Remove(e.OldValue);
            _valueControl.Items.Add(e.NewValue);
        }

        public override DependencyProperty ContentProperty => ComboBox.TextProperty;

        public override string Type => "keyedoption";

        private ComboBox CreateValueControlInternal()
        {
            var control = new ComboBox { ToolTip = Description, IsEditable = true };
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

            ValueAdded.Invoke(this, new ValueChangedEventArgs<string>(_storeKey, _currentValue, _valueControl.Text));
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