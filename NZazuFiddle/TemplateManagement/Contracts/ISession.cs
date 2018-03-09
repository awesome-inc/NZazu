using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel;


namespace NZazuFiddle.TemplateManagement.Contracts
{
    interface ISession : INotifyPropertyChanged
    {

        string Endpoint { get; }
        ISample SelectedSample { get; }
        List<ISample> Samples { get; }

        // ToDo: Solve via cental/global EventAggregator and Events handled directly
        // ToDo: Move to a specific Sample Repo
        void AddSampleAsUniqueItem(ISample sample);
        void AddSamplesAsUniqueItems(List<ISample> samples);
        bool DoesSampleAlreadyExist(ISample sample);
        void Replace(ISample sample);
        void ClearSamples();

    }
}
