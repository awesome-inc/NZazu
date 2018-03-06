using System;
using System.ComponentModel;
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
            _session.PropertyChanged += new PropertyChangedEventHandler(session_PropertyChanged);
        }

        public void setDbConfig(string endpoint)
        {
            _dbClient.Endpoint = endpoint;
        }

        /// <summary>
        /// Data binding with current central session via property change events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void session_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Trace.TraceInformation($"{GetType().Name}:: A session property has changed: {e.PropertyName}");
            if (e.PropertyName == "DbEndpoint" && _dbClient.Endpoint != _session.Endpoint) _dbClient.Endpoint = _session.Endpoint;
        }

        public void LoadTemplateFromFile(string pathToTemplate)
        {
            try
            {
                var loadedSample = _fileIo.LoadTemplateFromFile(pathToTemplate);
                _session.Samples.Add(loadedSample);
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
            //try
            //{
            //    var currentSamples = _session.Samples;
            //    currentSamples.Add(loadedSample);
            //    _session.Samples = currentSamples;
            //}
            //catch (Exception e)
            //{
            //    Trace.TraceError($"{this.GetType().Name}::LoadTemplateFromUri => \r\n Message:{e.Message} \r\n StackTrace:{e.StackTrace}");
            //}
        }

        public void ExportTemplatesToFolder(string pathToTemplateFolder)
        {
            var files = Directory.GetFiles(pathToTemplateFolder);
            foreach (string file in files)
            {
                LoadTemplateFromFile(file);
            }
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

        public void UpdateTemplateOnDb(string id)
        {
            var samples = _session.Samples;
            _dbClient.UpdateData(samples.First(t => t.Id == id));
        }

        public void CreateTemplateOnDb(string id)
        {
            throw new NotImplementedException();
        }

        public async void GetTemplatesFromDb()
        {
            var samplesFromTb = await _dbClient.GetData();
            _session.Samples.AddRange(samplesFromTb);
        }
    }
}
