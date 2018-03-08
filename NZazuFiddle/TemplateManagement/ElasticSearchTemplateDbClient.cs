using Newtonsoft.Json.Linq;
using NZazuFiddle.TemplateManagement.Contracts;
using NZazuFiddle.Utils;
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
                var res = await GetDataFromEndpoint(Endpoint + "_search?size=1000");
                var sampleList = DeserializeSamplesFromEndpoint(res);
                sampleList.ForEach(s => s.Status = ETemplateStatus.Initial);
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
                    sample.Status = ETemplateStatus.Initial;
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

        public async void DeleteData(ISample sample)
        {
            var id = sample.Id;
            try
            {   
                var endpoint = Endpoint + id;
                var response = await _httpClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var msg = LoggingUtil.CreateLogMessage(this, $"Deletion of data with {id} on endpoint {_endpoint} was successfull!");
                    Trace.TraceInformation(msg);
                }
                else
                {
                    var msg = LoggingUtil.CreateLogMessage(this, $"Deletion of data with {id} on endpoint {_endpoint} was NOT successfull!");
                    Trace.TraceInformation(msg);
                }

            }
            catch (Exception e)
            {
                var msg = LoggingUtil.CreateLogMessage(this, $"Deletion of data with {id} on endpoint {_endpoint} was NOT successfull!", e.Message, e.StackTrace);
                throw;
            }
        }
    }
}
