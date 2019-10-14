using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NZazu.JsonSerializer.RestSuggestor
{
    public interface IRestClient
    {
        Uri BaseAddress { get; set; }
        Task<JToken> Request(HttpMethod method = null, string uri = null, JToken body = null);
    }
}