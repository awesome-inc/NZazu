using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        public string Endpoint
        {
            get => _dbEndpoint;
            set { _dbEndpoint = value; OnPropertyChanged(nameof(Endpoint)); }
        }

        public ISample SelectedSample { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public BindableCollection<ISample> Samples {
            get => _samples;
        }

        public Session(string dbEndpoint, List<ISample> samples) {
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
            var sampleExist = !(_samples.FirstOrDefault(s => s.Id == sample.Id) == null);
            if(!sampleExist)
            {
                _samples.Add(sample);
            } else
            {
                Trace.TraceWarning(LoggingUtil.CreateLogMessage(this,$"Sample with Id {sample.Id} already exists!"));
            }
        }

        public void AddSamplesAsUniqueItems(List<ISample> samples)
        {
            samples.ForEach(AddSampleAsUniqueItem);
        }
    }
}
