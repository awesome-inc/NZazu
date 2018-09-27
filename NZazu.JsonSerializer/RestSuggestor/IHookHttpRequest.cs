using System.Net.Http;

namespace NZazu.JsonSerializer.RestSuggestor
{
    public interface IHookHttpRequest
    {
        void Hook(HttpRequestMessage request);
    }
}