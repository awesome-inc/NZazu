using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NZazu.JsonSerializer.RestSuggestor
{
    public class HttpClientWrapper : IHttpClient
    {
        private HttpClient _httpClient = new HttpClient();
        public Uri BaseAddress
        {
            get { return _httpClient.BaseAddress; }
            set { SetBaseAddress(value); }
        }
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message)
        {
            return _httpClient.SendAsync(message);
        }
        private void SetBaseAddress(Uri value)
        {
            try
            {
                _httpClient.BaseAddress = value;
            }
            catch (InvalidOperationException)
            {
                _httpClient.Dispose();
                _httpClient = new HttpClient { BaseAddress = value };
            }
        }
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}