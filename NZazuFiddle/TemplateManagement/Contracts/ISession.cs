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

        // ToDo: Solve via cental/global EventAggregator and Events handled directly
        // ToDo: Move to a specific Sample Repo
        void AddSampleAsUniqueItem(ISample sample);

        void AddSamplesAsUniqueItems(List<ISample> samples);

    }
}
