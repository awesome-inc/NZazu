using Newtonsoft.Json.Linq;
using NZazuFiddle.TemplateManagement.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.TemplateManagement
{
    internal class ElasticSearchTemplateDbClient : ITemplateDbClient
    {

        private readonly HttpClient _httpClient = new HttpClient();
        private string _endpoint;

        public ElasticSearchTemplateDbClient(string endpoint = "http://localhost:9200/tacon/form/")
        {
            _endpoint = endpoint;
        }

        public string Endpoint
        {
            get => _endpoint;
            set { _endpoint = value;}
        }

        public async Task<List<ISample>> GetData()
        {
            try
            {
                var res = await GetDataFromEndpoint(Endpoint + "_search");
                var sampleList = DeserializeSamplesFromEndpoint(res);
                return sampleList;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.StackTrace);
                throw;
            }
        }

        public async void UpdateData(ISample sample)
        {
            try
            {
                var id = sample.Id;
                var dataAsJson = MappingUtil.MapSampleToJson(sample);

                var content = new StringContent(dataAsJson, Encoding.UTF8, "application/json");
                var endpoint = Endpoint + id;
                var response = await _httpClient.PutAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    Trace.TraceInformation($"Uploading data {id} to endpoint {_endpoint} was successfull!");
                }
                else
                {
                    Trace.TraceWarning($"Uploading data {id} to endpoint {_endpoint} failed with status code: {response.StatusCode}");
                }

            }
            catch (Exception e)
            {
                Trace.TraceError($"{GetType().Name}:: {e.Message} \r\n {e.StackTrace}");
                throw;
            }
        }

        public void DeleteData(string id)
        {
            throw new NotImplementedException();
        }

        public void CreateData(ISample sample)
        {
            throw new NotImplementedException();
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

        private static List<ISample> DeserializeSamplesFromEndpoint(string json)
        {

            var samples = JObject.Parse(json)["hits"]
                    .SelectToken("hits")
                    .Select(hit => (dbId: hit.SelectToken("_id"), dbSource: hit.SelectToken("_source")))
                    .Select(source => MappingUtil.MapJsonToSample(
                            source.dbId.ToString(),
                            source.dbSource.SelectToken("Id").ToString(),
                            source.dbSource.SelectToken("FormDefinition").ToString(),
                            source.dbSource.SelectToken("Values").ToString()
                        )
                    )
                ;

            return samples.ToList();
        }


    }
}
