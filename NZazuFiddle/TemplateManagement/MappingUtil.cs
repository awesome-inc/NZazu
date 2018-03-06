using Newtonsoft.Json;
using NZazu.Contracts;
using NZazuFiddle.TemplateManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.TemplateManagement
{
    internal static class MappingUtil
    {

        public static ISample MapJsonToSample(string dbId, string sampleId, string sampleFormDefAsJson, string sampleFormValues)
        {
            var sampleFormDefinition = JsonConvert.DeserializeObject<FormDefinition>(sampleFormDefAsJson);
            var sampleFormData = new FormData(JsonConvert.DeserializeObject<Dictionary<string, string>>(sampleFormValues));
            var sampleTemplate = new TemplateSample(dbId, sampleId, sampleFormDefinition, sampleFormData);
            return sampleTemplate.Sample;
        }

        public static string MapSampleToJson(ISample sample)
        {
            var esDoc = new JsonFormDocument
            {
                Id = sample.Id,
                FormDefinition = sample.Fiddle.Definition.Model,
                Values = sample.Fiddle.Data.Model.Values
            };

            var sampleAsJsonForElasticSearch = JsonConvert.SerializeObject(esDoc, Formatting.Indented
                , new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

            return sampleAsJsonForElasticSearch;
        }

    }
}
