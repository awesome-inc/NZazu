using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        public void LoadTemplateFromFile(Uri pathToTemplate)
        {
            throw new NotImplementedException();
        }

        public void LoadTemplatesFromFolder(Uri pathToTemplateFolder)
        {
            throw new NotImplementedException();
        }

        public void ExportTemplateToFile(Uri pathToTemplate)
        {
            throw new NotImplementedException();
        }

        public void ExportTemplatesToFolder(Uri pathToTemplateFolder)
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
