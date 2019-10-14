using System.Windows;
using NZazu.Contracts.Adapter;
using NZazu.Contracts.Suggest;

namespace NZazu.Fields.Controls
{
    public partial class GeoLocationBox
    {
        public GeoLocationBox()
        {
            InitializeComponent();

            UpdateControl(this, Value, GeoLocationSupport);
        }

        private static void UpdateControl(
            GeoLocationBox glb,
            NZazuCoordinate val,
            ISupportGeoLocationBox support)
        {
            glb.SetToCurrentLocation.Visibility = support != null && support.HasCurrentPosition
                ? Visibility.Visible
                : Visibility.Collapsed;

            glb.OpenInGeoApp.Visibility = support != null && support.CanOpenGeoApp
                ? Visibility.Visible
                : Visibility.Collapsed;

            glb.OpenInGeoApp.IsEnabled = val != null && support != null && support.CanOpenGeoApp;

            glb.LocationBox.IsEnabled = support != null;
            glb.LocationBox.Text = support?.ToString(val) ?? "no valid coordinate converter added";
        }

        internal void OpenInGeoAppClick(object sender, RoutedEventArgs e)
        {
            if (!(GeoLocationSupport?.CanOpenGeoApp ?? false)) return;
            GeoLocationSupport?.OpenGeoApp(Value);
        }

        internal void SetToCurrentLocationClick(object sender, RoutedEventArgs e)
        {
            if (!(GeoLocationSupport?.HasCurrentPosition ?? false)) return;
            Value = GeoLocationSupport?.CurrentPosition;
        }

        internal void LocationBoxLostFocus(object sender, RoutedEventArgs e)
        {
            // lat lon decimal formatter is always supported -even if null
            if (GeoLocationSupport == null)
                Value = NZazuCoordinate.Parse(LocationBox.Text.Trim());
            else
                Value = GeoLocationSupport.Parse(LocationBox.Text.Trim());
        }

        #region dependency properties: GeoLocationSupport

        public static readonly DependencyProperty GeoLocationSupportProperty = DependencyProperty.Register(
            "GeoLocationSupport", typeof(ISupportGeoLocationBox), typeof(GeoLocationBox),
            new PropertyMetadata(default(ISupportGeoLocationBox), GeoLocationSupportChangedCallback));

        private static void GeoLocationSupportChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GeoLocationBox glb)) return;
            var support = e.NewValue as ISupportGeoLocationBox;

            UpdateControl(glb, glb.Value, support);
        }

        public ISupportGeoLocationBox GeoLocationSupport
        {
            get => (ISupportGeoLocationBox) GetValue(GeoLocationSupportProperty);
            set => SetValue(GeoLocationSupportProperty, value);
        }

        #endregion

        #region dependency properties: Value

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(NZazuCoordinate), typeof(GeoLocationBox),
            new PropertyMetadata(default(NZazuCoordinate), Coordinate));

        private static void Coordinate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GeoLocationBox glb)) return;
            var val = e.NewValue as NZazuCoordinate;

            UpdateControl(glb, val, glb.GeoLocationSupport);
        }

        public NZazuCoordinate Value
        {
            get => (NZazuCoordinate) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        #region dependency properties: IsReadOnly

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(GeoLocationBox), new PropertyMetadata(false, IsReadOnlyChangedCallback));

        private static void IsReadOnlyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GeoLocationBox box)) return;
            box.LocationBox.IsReadOnly = (bool) e.NewValue;
        }

        public bool IsReadOnly
        {
            get => (bool) GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        #endregion
    }
}