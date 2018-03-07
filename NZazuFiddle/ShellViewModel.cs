using Caliburn.Micro;
using NZazuFiddle.TemplateManagement.Contracts;
using System.Collections.Generic;

using System.Linq;
using System.Windows;

namespace NZazuFiddle
{
    internal sealed class ShellViewModel : Screen, IShell
    {
        private readonly BindableCollection<ISample> _samples;
        private ISample _selectedSample;
        private readonly ISession _session;
        private readonly ITemplateDbClient _templateDbRepo;
        private readonly ITemplateFileIo _templateFileIo;

        public ShellViewModel(
            ITemplateFileIo templateFileIo, 
            ITemplateDbClient templateDbRepo, 
            IEndpointViewModel endpointViewModel, 
            IFileMenuViewModel fileMenuViewModel, 
            IAddTemplateViewModel addTemplateViewModel, 
            ISession session)
        {
            DisplayName = "TACON Template Editor";

            _samples = session.Samples;
            _session = session;
            _templateDbRepo = templateDbRepo;
            EndpointViewModel = endpointViewModel;
            FileMenuViewModel = fileMenuViewModel;
            AddTemplateViewModel = addTemplateViewModel;
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

        public void RemoveSelectedSample()
        {
            _samples.Remove(SelectedSample);
        }

        public void DeleteSelectedSample()
        {
            _templateDbRepo.DeleteData(SelectedSample);
            _samples.Remove(SelectedSample);
        }

        public void ExportSelectedSample()
        {
        }

        public IEndpointViewModel EndpointViewModel { get; }

        public IFileMenuViewModel FileMenuViewModel { get; }

        public IAddTemplateViewModel AddTemplateViewModel { get; }

    }
}