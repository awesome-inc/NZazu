using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.TemplateManagement.Contracts
{
    interface ISession : INotifyPropertyChanged
    {

        string Endpoint { get; set; }

        ISample SelectedSample { get; set; }

        BindableCollection<ISample> Samples { get; }

    }
}
