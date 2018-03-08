using Caliburn.Micro;
using FontAwesome.Sharp;
using NZazuFiddle.TemplateManagement.Contracts;
using System;
using System.Diagnostics;
using System.Windows;

namespace NZazuFiddle
{
    internal class EndpointViewModel : Screen, IEndpointViewModel
    {
        private readonly ITemplateDbClient _templateDbRepo;
        private string _endpoint = "http://localhost:9200/tacon/form";
        private readonly ISession _session;

        public EndpointViewModel(ITemplateDbClient templateDbRepo, ISession session)
        {
            _templateDbRepo = templateDbRepo ?? throw new ArgumentNullException(nameof(templateDbRepo));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _endpoint = _templateDbRepo.Endpoint;
        }

        public string Endpoint
        {
            get => _endpoint;
            set
            {
                if (_endpoint == value) return;
                _endpoint = value;
                _templateDbRepo.Endpoint = value;
                NotifyOfPropertyChange(nameof(Endpoint));
                NotifyOfPropertyChange(nameof(CanLoadSamples));
                NotifyOfPropertyChange(nameof(CanSaveSamples));
            }
        }

        public bool CanLoadSamples { get => !string.IsNullOrWhiteSpace(Endpoint) && Uri.TryCreate(Endpoint, UriKind.Absolute, out Uri uri); }
        public bool CanSaveSamples { get => !string.IsNullOrWhiteSpace(Endpoint) && Uri.TryCreate(Endpoint, UriKind.Absolute, out Uri uri); }

        public async void LoadSamples()
        {
            try
            {
                var samplesFromDb = await _templateDbRepo.GetData();
                _session.AddSamplesAsUniqueItems(samplesFromDb);
            } catch(Exception e)
            {
                MessageBox.Show(
                            $"Endpoint not reachable. Please check if you are connected to the database or if the given URL is correct.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
            }
        }

        public void SaveSamples()
        {
            try
            {
                _session
                    .Samples
                    .Apply(data => {
                        _templateDbRepo.UpdateData(data);
                        });
            }
            catch (Exception e)
            {
                Trace.TraceError(e.StackTrace);
                MessageBox.Show("Upload failed!");
            }
        }
    }
}
