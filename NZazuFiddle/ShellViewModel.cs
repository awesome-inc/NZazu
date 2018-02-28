using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NZazu.Contracts;
using NZazuFiddle.Samples;

namespace NZazuFiddle
{
    internal sealed class ShellViewModel : Screen,IShell
    {
        private readonly BindableCollection<ISample> _samples = new BindableCollection<ISample>();
        private ISample _selectedSample;
        private string _endpointUri = "http://sw-mews-01-app:9200/tacon/form/_search";

        public ShellViewModel(IEnumerable<IHaveSample> samples = null)
        {
            DisplayName = "NZazuFiddle";
            if (samples != null) 
                Samples = samples.OrderBy(s => s.Order).Select(s => s.Sample);
        }

        public IEnumerable<ISample> Samples
        {
            get { return _samples; }
            set
            {
                if (Equals(value, _samples)) return;
                _samples.Clear();
                if (value != null) _samples.AddRange(value);
                SelectedSample = _samples.FirstOrDefault();
            }
        }

        public ISample SelectedSample
        {
            get { return _selectedSample; }
            set
            {
                if (Equals(value, _selectedSample)) return;
                _selectedSample = value;
                NotifyOfPropertyChange();
            }
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
            try
            {
                var res = await GetDataFromEndpoint(_endpointUri);
                var sampleList = DeserializeSamplesFromEndpoint(res);
                Samples = sampleList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }


        public List<ISample> DeserializeSamplesFromEndpoint(string json)
        {

            dynamic data = JsonConvert.DeserializeObject<Object>(json);
            var hits = data.hits.hits;



            //var sampleFormDefinition = JsonConvert.DeserializeObject<FormDefinition>(value);
            //var sampleFormData = JsonConvert.DeserializeObject<FormData>(value);

            //new SampleTemplate(sampleFormDefinition, sampleFormData);

            throw new NotImplementedException();
        }


    }
}