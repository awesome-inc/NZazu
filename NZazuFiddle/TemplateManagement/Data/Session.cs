using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using NZazuFiddle.Annotations;
using NZazuFiddle.TemplateManagement.Contracts;
using NZazuFiddle.Utils;

namespace NZazuFiddle.TemplateManagement.Data
{
    internal class Session : ISession
    {

        private string _dbEndpoint;
        private BindableCollection<ISample> _samples;
        private readonly IEventAggregator _events;

        public string Endpoint
        {
            get => _dbEndpoint;
            set { _dbEndpoint = value; OnPropertyChanged(nameof(Endpoint)); }
        }

        public ISample SelectedSample { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public BindableCollection<ISample> Samples {
            get => _samples;
        }

        public Session(string dbEndpoint, List<ISample> samples, IEventAggregator events) {
            _dbEndpoint = dbEndpoint;
            _samples = new BindableCollection<ISample>(samples);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddSampleAsUniqueItem(ISample sample)
        {
            HandleAlreadyExistingSample(sample);
        }

        public void AddSamplesAsUniqueItems(List<ISample> samples)
        {
            samples.ForEach(AddSampleAsUniqueItem);
        }

        public bool DoesSampleAlreadyExist(ISample sample)
        {
            var sampleExists = !(_samples.FirstOrDefault(s => s.Id == sample.Id) == null);
            return sampleExists;
        }

        public void Replace(ISample sample)
        {
            var toBeRemoved = _samples.FirstOrDefault(s => sample.Id == s.Id);
            if(toBeRemoved != null)
            {
                _samples.Remove(toBeRemoved);
                _samples.Add(sample);
            }

        }

        private void HandleAlreadyExistingSample(ISample sample)
        {
            var sampleExists = DoesSampleAlreadyExist(sample);
            if (sampleExists)
            {
                Trace.TraceWarning(LoggingUtil.CreateLogMessage(this, $"Sample with Id {sample.Id} already exists!"));

                var r = MessageBox.Show(
                    $"The template {sample.Name} does already exist in current local list. Do you want to replace it?",
                    "Already exists",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                    );

                if (r == MessageBoxResult.Yes)
                {
                    Replace(sample);
                } else
                {
                    Trace.TraceInformation(LoggingUtil.CreateLogMessage(this, $"Sample with Id {sample.Id} was not replaced!"));
                }
            } else
            {
                _samples.Add(sample);
            }
        }
    }
}
