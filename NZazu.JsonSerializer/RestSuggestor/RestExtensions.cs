using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NZazu.JsonSerializer.RestSuggestor
{
    public static class RestExtensions
    {
        public static async Task<JToken> Head(this IRestClient client, string uri = null)
        {
            return await client.Request(HttpMethod.Head, uri);
        }

        public static async Task<JToken> Get(this IRestClient client, string uri = null)
        {
            return await client.Request(HttpMethod.Get, uri);
        }

        public static async Task<JToken> Post(this IRestClient client, string uri, JToken body = null)
        {
            return await client.Request(HttpMethod.Post, uri, body);
        }

        public static async Task<JToken> Put(this IRestClient client, string uri, JToken body = null)
        {
            return await client.Request(HttpMethod.Put, uri, body);
        }

        public static async Task<JToken> Delete(this IRestClient client, string uri, JToken body = null)
        {
            return await client.Request(HttpMethod.Delete, uri, body);
        }
    }
}