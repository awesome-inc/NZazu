using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        public void ExportTemplate(ISample sample, string pathToFile)
        {
            try
            {
                //File.Create(pathToFile);
                var sampleAsJson = MappingUtil.MapSampleToJson(sample);
                File.WriteAllText(pathToFile, sampleAsJson);
            }
            catch (Exception e)
            {
                Trace.TraceError($"{GetType().Name} :: ExportTemplate for sample with target path {pathToFile} failed! \r\n {e.Message} \r\n {e.StackTrace}");
                throw;
            }

        }

        public void ExportTemplates(List<ISample> samples, string pathToFolder)
        {
            foreach (var sample in samples)
            {
                var fileName = sample.Id + ".json";
                var fullPath = pathToFolder + Path.DirectorySeparatorChar + fileName;
                ExportTemplate(sample,fullPath);
            }
        }

        public ISample LoadTemplateFromFile(string pathToTemplate)
        {
            ISample loadedSample;
            using (StreamReader r = new StreamReader(pathToTemplate))
            {
                var json = r.ReadToEnd();
                loadedSample = DeserializeSampleFromJSONFile(json);
                loadedSample.Status = ETemplateStatus.Imported;
            }    

            return loadedSample;
        }

        public List<ISample> LoadTemplatesFromFolder(string pathToFolder)
        {
            var files = Directory.GetFiles(pathToFolder);
            var listOfSamples = new List<ISample>();
            foreach (string file in files)
            {
                var loadedSample = LoadTemplateFromFile(file);
                loadedSample.Status = ETemplateStatus.Imported;
                listOfSamples.Add(loadedSample);
            }
            return listOfSamples;
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
