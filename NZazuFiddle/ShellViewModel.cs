using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FluentAssertions.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NZazu.Contracts;
using NZazuFiddle.Samples;
using Xceed.Wpf.Toolkit;

namespace NZazuFiddle
{
    internal sealed class ShellViewModel : Screen, IShell
    {
        private readonly BindableCollection<ISample> _samples = new BindableCollection<ISample>();
        private ISample _selectedSample;

        // REST communication with Elasticsearch
        private string _endpointUri = "http://127.0.0.1:9200/tacon/form/";
        private readonly HttpClient _httpClient = new HttpClient();

        public ShellViewModel(IEnumerable<IHaveSample> samples = null)
        {
            DisplayName = "Tacon Template Editor";
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

        private async Task<string> GetDataFromEndpoint(string endpoint)
        {
            string result = null;          
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }
            else
            {
                Trace.TraceWarning($"Loading data from endpoint failed with status code: {response.StatusCode}");
            }
            return result;
        }

        private async void UploadDataToEndpoint(string requestUri, string id, string dataAsJson)
        {
            var content = new StringContent(dataAsJson, Encoding.UTF8, "application/json");
            var endpoint = requestUri + id;
            var response = await _httpClient.PutAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                Trace.TraceInformation("UploadDataToEndpoint");
            }
            else
            {
                Trace.TraceWarning($"Uploading data from endpoint failed with status code: {response.StatusCode}");
            }

        }

        public async void GetData()
        {
            try
            {
                var res = await GetDataFromEndpoint(_endpointUri + "_search");
                var sampleList = DeserializeSamplesFromEndpoint(res);
                Samples = sampleList;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.StackTrace);
                throw;
            }

        }

        public void SendData()
        {
            try
            {
                Samples
                    .ToList()
                    .Select(sample => (id: sample.Id, json: MapSampleToJson(sample)))
                    .Apply(data => UploadDataToEndpoint(_endpointUri, data.id, data.json));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.StackTrace);
                MessageBox.Show("Upload failed!");
            }
        }

        private static List<ISample> DeserializeSamplesFromEndpoint(string json)
        {

            var samples = JObject.Parse(json)["hits"]
                .SelectToken("hits")
                .Select(hit => (dbId: hit.SelectToken("_id"), dbSource: hit.SelectToken("_source")))
                .Select(source => MapJsonToSample(
                        source.dbId.ToString(),
                        source.dbSource.SelectToken("Id").ToString(),
                        source.dbSource.SelectToken("FormDefinition").ToString(),
                        source.dbSource.SelectToken("Values").ToString()
                        )
                    )
                ;

            return samples.ToList();
        }

        private static ISample MapJsonToSample(string dbId, string sampleId, string sampleFormDefAsJson, string sampleFormDataAsJson)
        {
            var sampleFormDefinition = JsonConvert.DeserializeObject<FormDefinition>(sampleFormDefAsJson);
            var sampleFormData = JsonConvert.DeserializeObject<FormData>(sampleFormDataAsJson);
            var sampleTemplate = new SampleTemplate(dbId, sampleId, sampleFormDefinition, sampleFormData);
            return sampleTemplate.Sample;
        }

        private static string MapSampleToJson(ISample sample)
        {
            //var formDefinitionAsJson = sample.Fiddle.Definition.Json;
            //var formDataAsJson = sample.Fiddle.Data.Json;
            //var id = sample.Name;

            //var sourceUpdate = "{\r\n" + $"\"Id\": \"{id}\", \r\n {formDefinitionAsJson}, \r\n {formDataAsJson} \r\n" + "}";
            //return sourceUpdate;

            var esDoc = new ElasticSearchDocument
            {
                Id = sample.Id,
                FormDefinition = sample.Fiddle.Definition.Model,
                Values = sample.Fiddle.Data.Model
            };

            var sampleAsJsonForElasticSearch = JsonConvert.SerializeObject(esDoc, Formatting.Indented
                , new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Ignore});

            return sampleAsJsonForElasticSearch;
        }

        private class ElasticSearchDocument
        {
            public string Id { get; set; }
            public FormDefinition FormDefinition { get; set; }
            public FormData Values { get; set; }
        }


    }
}