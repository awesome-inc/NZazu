using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NZazuFiddle.TemplateManagement.Contracts
{
    internal class TemplateRepoManager : ITemplateRepoManager
    {

        private readonly ITemplateDbClient _dbClient;
        private readonly ITemplateFileIo _fileIo;
        private readonly ISession _session;

        public TemplateRepoManager(ITemplateDbClient dbClient, ITemplateFileIo fileIo, ISession session)
        {
            _session = session;
            _dbClient = dbClient;
            _fileIo = fileIo;
        }

        public void setDbConfig(string endpoint)
        {
            _dbClient.Endpoint = endpoint;
        }

        public void LoadTemplateFromFile(string pathToTemplate)
        {
            try
            {
                var loadedSample = _fileIo.loadTemplate(pathToTemplate);
                var currentSamples = _session.Samples;
                currentSamples.Add(loadedSample);
                _session.Samples = currentSamples;
            } catch(Exception e)
            {
                Trace.TraceError($"{this.GetType().Name}::LoadTemplateFromUri => \r\n Message:{e.Message} \r\n StackTrace:{e.StackTrace}");
            }
        }

        public void LoadTemplatesFromFolder(string pathToTemplateFolder)
        {
            var files = Directory.GetFiles(pathToTemplateFolder);
            foreach(string file in files) {
                LoadTemplateFromFile(file);
            }
        }

        public void ExportTemplateToFile(string pathToTemplate)
        {
            throw new NotImplementedException();
        }

        public void ExportTemplatesToFolder(string pathToTemplateFolder)
        {
            throw new NotImplementedException();
        }

        public void CreateNewEmptyTemplate(string id)
        {
            throw new NotImplementedException();
        }

        public void RemoveTemplate(string id)
        {
            throw new NotImplementedException();
        }

        public void RemoveTemplateFromDb(string id)
        {
            throw new NotImplementedException();
        }

        public async void UpdateTemplateOnDb(string id)
        {
            var samples = _session.Samples;
            await _dbClient.UpdateData(samples.First(t => t.Id == id));
        }

        public void CreateTemplateOnDb(string id)
        {
            throw new NotImplementedException();
        }

        public async void GetTemplatesFromDb()
        {
            _session.Samples = await _dbClient.GetData();
        }
    }
}
