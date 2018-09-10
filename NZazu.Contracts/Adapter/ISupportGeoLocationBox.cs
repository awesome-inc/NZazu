using System.Threading.Tasks;

namespace NZazu.Contracts.Adapter
{
    public interface ISupportGeoLocationBox
    {
        bool HasCurrentPosition { get; }
        NZazuCoordinate CurrentPosition { get; }

        bool CanOpenGeoApp { get; }
        Task OpenGeoApp(NZazuCoordinate nZazuCoordinate);

        string ToString(NZazuCoordinate c);
        NZazuCoordinate Parse(string source);
    }
}
