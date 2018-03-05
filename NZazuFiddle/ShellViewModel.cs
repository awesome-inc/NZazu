using Caliburn.Micro;
using Microsoft.Win32;
using NZazuFiddle.Samples;
using NZazuFiddle.TemplateManagement;
using NZazuFiddle.TemplateManagement.Contracts;
using NZazuFiddle.TemplateManagement.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xceed.Wpf.Toolkit;

namespace NZazuFiddle
{
    internal sealed class ShellViewModel : Screen, IShell
    {
        private readonly BindableCollection<ISample> _samples = new BindableCollection<ISample>();
        private ISample _selectedSample;
        private string _endpoint;
        private readonly Session _session;

        private readonly ITemplateRepoManager _templateManager = new TemplateRepoManager(new ElasticSearchTemplateDbClient(), new JsonTemplateFileIo(), Session.Instance);

        public ShellViewModel(IEnumerable<IHaveSample> samples = null)
        {
            DisplayName = "Tacon Template Editor";
            if (samples != null)
                Samples = samples.OrderBy(s => s.Order).Select(s => s.Sample);

            _session = Session.Instance;
            _session.PropertyChanged += new PropertyChangedEventHandler(session_PropertyChanged);
            _session.Endpoint = "http://127.0.0.1:9200/tacon/form/";
        }

        /// <summary>
        /// Data binding with current central session via property change events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void session_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Trace.TraceInformation("ShellViewModel:: A session property has changed: " + e.PropertyName);
            if(e.PropertyName == "DbEndpoint") Endpoint = _session.Endpoint;
            if (e.PropertyName == "Samples") Samples = _session.Samples;
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

        // File handling
        public void ImportFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                var isUri = Uri.TryCreate(openFileDialog.FileName, UriKind.Absolute, out Uri uri);
                if(isUri) _templateManager.LoadTemplateFromFile(openFileDialog.FileName);
            }
        }

        public void ImportFiles()
        {
            var dialog = new OpenFolderService();
            var folder = dialog.SelectFolder();
            if (string.IsNullOrWhiteSpace(folder)) return;
            _templateManager.LoadTemplatesFromFolder(folder);
        }

        // Database communication
        public string Endpoint
        {
            get => _endpoint;
            set
            {
                if (Equals(value, _endpoint)) return;
                _endpoint = value;
            }

        }

        public void GetData()
        {
            _templateManager.GetTemplatesFromDb();
        }

        public void SendData()
        {
            try
            {
                Samples
                    .ToList()
                    .Apply(data => _templateManager.UpdateTemplateOnDb(data.Id));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.StackTrace);
                MessageBox.Show("Upload failed!");
            }
        }




    }
}