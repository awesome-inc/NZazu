using System.Threading.Tasks;

namespace NZazu.Contracts.Adapter
{
    public class SupportGeoLocationBox : ISupportGeoLocationBox
    {
        public bool HasCurrentPosition { get; } = false;
        public NZazuCoordinate CurrentPosition { get; } = null;
        public bool CanOpenGeoApp { get; } = false;

        public Task OpenGeoApp(NZazuCoordinate nZazuCoordinate)
        {
            throw new System.NotImplementedException();
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