using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NZazuFiddle.TemplateManagement.Contracts;

namespace NZazuFiddle.TemplateManagement
{
    internal class JsonTemplateFileIo : ITemplateFileIo
    {
        public void exportTemplate(Uri pathToFile)
        {
            throw new NotImplementedException();
        }

        public ISample loadTemplate(string pathToTemplate)
        {
            ISample loadedSample = null;
            using (StreamReader r = new StreamReader(pathToTemplate))
            {
                string json = r.ReadToEnd();
                loadedSample = DeserializeSampleFromJSONFile(json);
            }    

            return loadedSample;
        }

        private ISample DeserializeSampleFromJSONFile(string json)
        {

            var jsonObject = JObject.Parse(json);
            var sample = MappingUtil.MapJsonToSample(
                jsonObject.SelectToken("Id").ToString(), 
                jsonObject.SelectToken("Id").ToString(), 
                jsonObject.SelectToken("FormDefinition").ToString(),
                jsonObject.SelectToken("Values").ToString());
            
            return sample;
        }
    }
}
