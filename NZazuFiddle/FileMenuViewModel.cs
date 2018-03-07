using Caliburn.Micro;
using FontAwesome.Sharp;
using NZazuFiddle.TemplateManagement.Contracts;
using System;
using System.Linq;

namespace NZazuFiddle
{
    class FileMenuViewModel : Screen, IFileMenuViewModel
    {

        private readonly ISession _session;
        private readonly ITemplateFileIoDialogService _fileIo;

        public FileMenuViewModel(ITemplateFileIoDialogService fileIo, ISession session)
        {
            _fileIo = fileIo ?? throw new ArgumentNullException(nameof(fileIo));
            _session = session ?? throw new ArgumentNullException(nameof(session));

            ImportIcon = IconChar.LevelDown;
            ExportIcon = IconChar.HddO;
        }

        public IconChar ImportIcon { get; set; }

        public IconChar ExportIcon { get; set; }

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
    }
}
