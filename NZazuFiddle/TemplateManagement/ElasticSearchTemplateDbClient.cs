using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NZazu.Contracts;
using NZazuFiddle.TemplateManagement.Contracts;
using NZazuFiddle.TemplateManagement.Data;
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

        public string EndPoint { get => _endpoint; set => _endpoint = value; }

        public ElasticSearchTemplateDbClient(string endPoint = "http://127.0.0.1:9200/tacon/form/")
        {
            _endpoint = endPoint;
        }

        public string Endpoint { get; set; }

        public async Task<List<ISample>> GetData()
        {
            try
            {
                var res = await GetDataFromEndpoint(EndPoint + "_search");
                var sampleList = DeserializeSamplesFromEndpoint(res);
                return sampleList;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.StackTrace);
                throw;
            }

        }

        public async Task<string> UpdateData(ISample sample)
        {
            try
            {
                var id = sample.Id;
                var dataAsJson = MappingUtil.MapSampleToJson(sample);

                var content = new StringContent(dataAsJson, Encoding.UTF8, "application/json");
                var endpoint = EndPoint + id;
                var response = await _httpClient.PutAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    Trace.TraceInformation("UploadDataToEndpoint");
                }
                else
                {
                    Trace.TraceWarning($"Uploading data from endpoint failed with status code: {response.StatusCode}");
                }

                return response.ToString();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.StackTrace);
                throw;
            }
        }

        public string DeleteData(string id)
        {
            throw new NotImplementedException();
        }

        public string CreateData(ISample sample)
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
