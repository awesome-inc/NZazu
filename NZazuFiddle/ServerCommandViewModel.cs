using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle
{
    class ServerCommandViewModel : IServerCommand
    {
        private string _endpointUri = "http://sw-mews-01-app:9200/tacon";

        public string Endpoint
        {
            get => _endpointUri;
            set
            {
                if (Equals(value, _endpointUri)) return;
                _endpointUri = value;
            }

        }

        public async Task<string> GetDataFromEndpoint(string endpoint)
        {
            string result = null;

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }
    }
}
