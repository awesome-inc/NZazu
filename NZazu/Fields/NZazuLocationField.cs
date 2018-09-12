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
        private readonly ISupportGeoLocationBox _geoSupport;

        public NZazuLocationField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            _geoSupport = (ISupportGeoLocationBox)serviceLocatorFunc(typeof(ISupportGeoLocationBox));
        }

        public override void SetValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Value = _geoSupport.Parse(value);
            }
        }

        public override string GetValue()
        {
            if (Value == null || !Value.GetIsValid()) return string.Empty;
            // ReSharper disable PossibleInvalidOperationException
            return Value.Lat.ToString(CultureInfo.InvariantCulture) + ", " + Value.Lon.ToString(CultureInfo.InvariantCulture);
            // ReSharper enable PossibleInvalidOperationException
        }

        public override DependencyProperty ContentProperty => GeoLocationBox.ValueProperty;
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
