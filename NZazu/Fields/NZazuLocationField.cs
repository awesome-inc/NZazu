using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;
using NZazu.Contracts.Adapter;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    internal class NZazuLocationField : NZazuField<NZazuCoordinate>
    {
        private GeoLocationBox _valueControl;
        private ISupportGeoLocationBox _geoSupport;

        public override string Type => "location";

        public NZazuLocationField(FieldDefinition definition) : base(definition) { }

        public override NZazuField Initialize(Func<Type, object> propertyLookup)
        {
            _geoSupport = (ISupportGeoLocationBox)propertyLookup(typeof(ISupportGeoLocationBox));

            _valueControl = new GeoLocationBox
            {
                ToolTip = Description,
                GeoLocationSupport = _geoSupport
            };

            return base.Initialize(propertyLookup);
        }

        protected override void SetStringValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Value =_geoSupport.Parse(value);
            }
        }

        protected override string GetStringValue()
        {
            if (Value == null || !Value.GetIsValid()) return string.Empty;
            // ReSharper disable PossibleInvalidOperationException
            return Value.Lat.ToString(CultureInfo.InvariantCulture) + ", " + Value.Lon.ToString(CultureInfo.InvariantCulture);
            // ReSharper enable PossibleInvalidOperationException
        }

        public override DependencyProperty ContentProperty => GeoLocationBox.ValueProperty;
        protected override Binding DecorateBinding(Binding binding)
        {
            binding.TargetNullValue = null;
            return base.DecorateBinding(binding);
        }

        protected override Control CreateValueControl()
        {
            return _valueControl;
        }
    }
}
