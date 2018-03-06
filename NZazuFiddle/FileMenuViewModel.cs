using Caliburn.Micro;
using Microsoft.Win32;
using NZazuFiddle.TemplateManagement.Contracts;
using System;
using System.Diagnostics;
using System.Linq;

namespace NZazuFiddle
{
    class FileMenuViewModel : Screen, IFileMenuViewModel
    {

        private readonly ISession _session;
        private readonly ITemplateFileIo _fileIo;

        public FileMenuViewModel(ITemplateFileIo fileIo, ISession session)
        {
            _fileIo = fileIo ?? throw new ArgumentNullException(nameof(fileIo));
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public void ExportToFiles()
        {
            var dialog = new OpenFolderService();
            var folder = dialog.SelectFolder();
            if (string.IsNullOrWhiteSpace(folder)) return;
            _fileIo.ExportTemplates(_session.Samples.ToList(), folder);
        }

        public void ImportFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var isUri = Uri.TryCreate(openFileDialog.FileName, UriKind.Absolute, out Uri uri);
                if (isUri)
                {
                    try
                    {
                        var loadedSample = _fileIo.LoadTemplateFromFile(openFileDialog.FileName);
                        _session.AddSampleAsUniqueItem(loadedSample);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError($"{this.GetType().Name}::LoadTemplateFromUri => \r\n Message:{e.Message} \r\n StackTrace:{e.StackTrace}");
                    }
                }
            }
        }

        public void ImportFiles()
        {
            var dialog = new OpenFolderService();
            var folder = dialog.SelectFolder();
            if (string.IsNullOrWhiteSpace(folder)) return;
            var loadedSamples = _fileIo.LoadTemplatesFromFolder(folder);
            _session.AddSamplesAsUniqueItems(loadedSamples);
        }
    }
}
