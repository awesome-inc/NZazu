using System;
using System.Globalization;

namespace NZazu.Contracts.Adapter
{
    /// <summary>
    ///     GeoPoint used for location mapping.
    ///     cf: http://stackoverflow.com/questions/31088120/c-sharp-nest-how-to-index-array-of-geo-poinst
    /// </summary>
    public class NZazuCoordinate
    {
        private const double LatMin = -90;
        private const double LonMin = -180;
        private const double LatMax = +90;
        private const double LonMax = +180;

        public double Lat { get; set; }
        public double Lon { get; set; }

        // methods so these dont get serialized
        public bool GetIsValid()
        {
            return Lat >= LatMin && Lat <= LatMax &&
                   Lon >= LonMin && Lon <= LonMax;
        }

        public static NZazuCoordinate Parse(string latLonDecimalWithSpace)
        {
            var split = latLonDecimalWithSpace
                .Replace(',', '.') // german comma is allowed
                .Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 2) return null;

            var canLat = double.TryParse(split[0], NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                NumberFormatInfo.InvariantInfo, out var lat);
            var canLon = double.TryParse(split[1], NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                NumberFormatInfo.InvariantInfo, out var lon);
            if (!canLat || !canLon) return null;

            var result = new NZazuCoordinate {Lat = lat, Lon = lon};
            if (!result.GetIsValid()) return null;

            return result;
        }

        public override string ToString()
        {
            var la = Lat.ToString(CultureInfo.InvariantCulture);
            var lo = Lon.ToString(CultureInfo.InvariantCulture);
            return $"{la} {lo}";
        }
    }
}