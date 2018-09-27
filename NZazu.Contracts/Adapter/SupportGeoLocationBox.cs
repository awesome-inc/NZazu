using System.Diagnostics;
using System.Threading.Tasks;
using NZazu.Contracts.Suggest;

namespace NZazu.Contracts.Adapter
{
    public class SupportGeoLocationBox : ISupportGeoLocationBox
    {
        public virtual bool HasCurrentPosition { get; } = false;

        public virtual NZazuCoordinate CurrentPosition { get; } = null;

        public bool CanOpenGeoApp { get; } = true;

        public Task OpenGeoApp(NZazuCoordinate c)
        {
            return Task.Run(() => Process.Start($"https://www.google.com/maps/place/@{c.Lat},{c.Lon},14z"));
        }

        public string ToString(NZazuCoordinate c)
        {
            return c?.ToString() ?? string.Empty;
        }

        public NZazuCoordinate Parse(string source)
        {
            if (string.IsNullOrEmpty(source)) return null;
            return NZazuCoordinate.Parse(source);
        }
    }
}