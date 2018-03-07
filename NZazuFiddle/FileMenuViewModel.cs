using Caliburn.Micro;
using FontAwesome.Sharp;
using NZazu.Contracts;
using NZazuFiddle.TemplateManagement;
using NZazuFiddle.TemplateManagement.Contracts;
using System;
using System.Linq;

namespace NZazuFiddle
{
    class FileMenuViewModel : Screen, IFileMenuViewModel
    {

        private readonly ISession _session;
        private readonly ITemplateFileIoDialogService _fileIo;
        private readonly IWindowManager _windowManager;

        public FileMenuViewModel(ITemplateFileIoDialogService fileIo, ISession session, IWindowManager windowManager)
        {
            _fileIo = fileIo ?? throw new ArgumentNullException(nameof(fileIo));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));

            ImportIcon = IconChar.LevelDown;
            ExportIcon = IconChar.HddO;
            NewFileIcon = IconChar.Plus;
        }

        public IconChar ImportIcon { get; set; }

        public IconChar ExportIcon { get; set; }

        public IconChar NewFileIcon { get; set; }

        public void ExportToFiles()
        {
            _fileIo.ExportTemplates(_session.Samples.ToList());
        }

        public void ImportFile()
        {
            var sampleTemplate = _fileIo.ImportTemplateFromFile();
            _session.AddSampleAsUniqueItem(sampleTemplate);
        }

        public void ImportFiles()
        {
            var loadedSamples = _fileIo.ImportTemplatesFromFolder();
            _session.AddSamplesAsUniqueItems(loadedSamples);
        }

        public void NewFile()
        {
            var newFileViewModel = new NewFileViewModel();
            _windowManager.ShowDialog(newFileViewModel);
            if (newFileViewModel.IsCancelled)
            {
                // Handle cancellation
            }
            else
            {
                var newTemplateSample = new TemplateSample(newFileViewModel.SampleId, newFileViewModel.SampleId, new FormDefinition(), new FormData(), ETemplateStatus.New);
                _session.AddSampleAsUniqueItem(newTemplateSample.Sample);
            }
        }

    }
}
