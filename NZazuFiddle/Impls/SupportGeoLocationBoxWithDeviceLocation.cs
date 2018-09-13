using System.Device.Location;
using NZazu.Contracts.Adapter;

namespace NZazuFiddle.Impls
{
    public class SupportGeoLocationBoxWithDeviceLocation
    : SupportGeoLocationBox
    {
        private readonly GeoCoordinateWatcher _watcher;

        public SupportGeoLocationBoxWithDeviceLocation()
        {
            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
        }

        public override bool HasCurrentPosition => _watcher.Permission == GeoPositionPermission.Granted;

        public override NZazuCoordinate CurrentPosition
        {
            get
            {
                var p = _watcher.Position;
                if (p.Location.IsUnknown) return null;

                return new NZazuCoordinate()
                {
                    Lat = p.Location.Latitude,
                    Lon = p.Location.Longitude
                };
            }
        }
    }
}
