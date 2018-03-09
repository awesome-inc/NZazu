using Caliburn.Micro;
using FontAwesome.Sharp;
using NZazu.Contracts;
using NZazuFiddle.TemplateManagement;
using NZazuFiddle.TemplateManagement.Contracts;
using System;
using System.Linq;
using System.Windows;

namespace NZazuFiddle
{
    class FileMenuViewModel : Screen, IFileMenuViewModel
    {

        private readonly ISession _session;
        private readonly ITemplateFileIoDialogService _fileIo;
        private readonly IWindowManager _windowManager;
        private readonly ITemplateDbClient _templateDbRepo;

        public FileMenuViewModel(ITemplateFileIoDialogService fileIo, ISession session, IWindowManager windowManager, ITemplateDbClient templateDbRepo)
        {
            _fileIo = fileIo ?? throw new ArgumentNullException(nameof(fileIo));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
            _templateDbRepo = templateDbRepo ?? throw new ArgumentNullException(nameof(templateDbRepo));

            ImportIcon = IconChar.FolderOpenO;
            ExportIcon = IconChar.HddO;
            NewFileIcon = IconChar.FileO;
            DeleteAllIcon = IconChar.TrashO;
            ClearAllIcon = IconChar.Eraser;
        }

        public IconChar ImportIcon { get; set; }

        public IconChar ExportIcon { get; set; }

        public IconChar NewFileIcon { get; set; }

        public IconChar DeleteAllIcon { get; set; }

        public IconChar ClearAllIcon { get; set; }

        public void ExportToFiles()
        {
            _fileIo.ExportTemplates(_session.Samples);
        }

        public void ImportFile()
        {
            var sampleTemplate = _fileIo.ImportTemplateFromFile();
            if(sampleTemplate != null) _session.AddSampleAsUniqueItem(sampleTemplate);
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
            if (!newFileViewModel.IsCancelled)
            {
                var newTemplateSample = new TemplateSample(newFileViewModel.SampleId, newFileViewModel.SampleId, new FormDefinition(), new FormData(), ETemplateStatus.New);
                _session.AddSampleAsUniqueItem(newTemplateSample.Sample);
            }
        }

        public void DeleteAll()
        {
            var r = MessageBox.Show(
                "Do you really want to delete all templates from database?",
                "Delete all",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (r == MessageBoxResult.Yes)
            {
                foreach(ISample sample in _session.Samples)
                {
                    _templateDbRepo.DeleteData(sample);
                }
                _session.ClearSamples();
            }
        }

        public void ClearAll()
        {
            var r = MessageBox.Show(
                "Do you really want to clear the current local list? " +
                "All local not synchronized changes will be lost.",
                "Clear list",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (r == MessageBoxResult.Yes)
            {
                _session.ClearSamples();
            }
        }

    }
}
