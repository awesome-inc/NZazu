using System.Threading.Tasks;
using NZazu.Contracts.Adapter;

namespace NZazu.Contracts.Suggest
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