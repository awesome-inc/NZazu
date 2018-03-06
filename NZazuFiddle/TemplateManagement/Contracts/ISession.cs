using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel;


namespace NZazuFiddle.TemplateManagement.Contracts
{
    interface ISession : INotifyPropertyChanged
    {

        string Endpoint { get; set; }

        ISample SelectedSample { get; set; }

        BindableCollection<ISample> Samples { get; }

        // ToDo: Solve via cental EventAggregator and Events
        void AddSampleAsUniqueItem(ISample sample);

        void AddSamplesAsUniqueItems(List<ISample> samples);

    }
}
