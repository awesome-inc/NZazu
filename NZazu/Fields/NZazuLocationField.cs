using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;
using NZazu.Contracts.Adapter;
using NZazu.Contracts.Suggest;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    internal class NZazuLocationField : NZazuField<NZazuCoordinate>
    {
        private readonly ISupportGeoLocationBox _geoSupport;

        public NZazuLocationField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            _geoSupport = (ISupportGeoLocationBox) serviceLocatorFunc(typeof(ISupportGeoLocationBox));
        }

        public override DependencyProperty ContentProperty => GeoLocationBox.ValueProperty;

        public override void SetValue(string newValue)
        {
            Value = string.IsNullOrWhiteSpace(newValue)
                ? null
                : _geoSupport.Parse(newValue);
        }

        public override string GetValue()
        {
            if (Value == null || !Value.GetIsValid()) return string.Empty;
            return _geoSupport.ToString(Value);
        }

        protected internal override Binding DecorateBinding(Binding binding)
        {
            binding.TargetNullValue = null;
            return base.DecorateBinding(binding);
        }

        protected override Control CreateValueControl()
        {
            return new GeoLocationBox
            {
                ToolTip = Definition.Description,
                GeoLocationSupport = _geoSupport
            };
        }
    }
}