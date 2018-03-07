using Microsoft.Win32;
using NZazuFiddle.TemplateManagement.Contracts;
using NZazuFiddle.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.TemplateManagement
{
    class TemplateFileIoDialogService : ITemplateFileIoDialogService
    {

        private readonly ITemplateFileIo _fileIo;

        public TemplateFileIoDialogService(ITemplateFileIo fileIo)
        {
            _fileIo = fileIo;
        }

        public void ExportTemplate(ISample sample)
        {
            var samples = new List<ISample>
            {
                sample
            };
            ExportTemplates(samples);
        }

        public void ExportTemplates(List<ISample> samples)
        {
            var dialog = new OpenFolderService();
            var folder = dialog.SelectFolder();
            if (string.IsNullOrWhiteSpace(folder)) return;
            _fileIo.ExportTemplates(samples, folder);
        }

        public ISample ImportTemplateFromFile()
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
                        return loadedSample;
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(LoggingUtil.CreateLogMessage(this, "Importing file failed!", e.Message, e.StackTrace));
                    }
                }
            }
            return null;
        }

        public List<ISample> ImportTemplatesFromFolder()
        {
            var dialog = new OpenFolderService();
            var folder = dialog.SelectFolder();
            if (string.IsNullOrWhiteSpace(folder)) return null;
            var loadedSamples = _fileIo.LoadTemplatesFromFolder(folder);
            return loadedSamples;
        }
    }
}
