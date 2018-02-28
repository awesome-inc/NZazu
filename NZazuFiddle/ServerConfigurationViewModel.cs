using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;

namespace NZazuFiddle
{
    class ServerConfigurationViewModel : IServerConfiguration
    {
        private readonly IEventAggregator _events = null;
        private string _endpointUri = "http://sw-mews-01-app:9200/tacon";

        public ServerConfigurationViewModel(IEventAggregator events, string endpointUri = "http://sw-mews-01-app:9200/tacon")
        {
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _events.Subscribe(this);
            _endpointUri = endpointUri ?? throw new ArgumentNullException(nameof(endpointUri));
        }

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

        public async void GetData()
        {
            var res = await GetDataFromEndpoint(_endpointUri);

            _events.PublishOnUIThread(res);
        }
    
    }
}
