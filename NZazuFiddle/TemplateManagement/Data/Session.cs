using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly List<ISample> _samples;

        public string Endpoint
        {
            get => _dbEndpoint;
            set { _dbEndpoint = value; OnPropertyChanged(nameof(Endpoint)); }
        }

        public ISample SelectedSample { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<ISample> Samples {
            // assure immutability of sample list
            get => new List<ISample>(_samples);
        }

        public Session(string dbEndpoint, List<ISample> samples) {
            _dbEndpoint = dbEndpoint;
            _samples = new List<ISample>(samples);
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        // ReSharper disable once MemberCanBePrivate.Global
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddSampleAsUniqueItem(ISample sample)
        {
            HandleAlreadyExistingSample(sample);
            OnPropertyChanged(nameof(Samples));
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
            OnPropertyChanged(nameof(Samples));
        }

        public void ClearSamples()
        {
            _samples.Clear();
            OnPropertyChanged(nameof(Samples));
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
