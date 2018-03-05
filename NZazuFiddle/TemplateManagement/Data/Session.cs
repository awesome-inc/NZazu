using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NZazuFiddle.Annotations;
using NZazuFiddle.TemplateManagement.Contracts;

namespace NZazuFiddle.TemplateManagement.Data
{
    internal class Session : ISession, INotifyPropertyChanged
    {
        private static Session _instance;
        public static Session Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Session();
                }
                return _instance;
            }
        }

        private string _dbEndpoint;
        private List<ISample> _samples;

        public string Endpoint
        {
            get => _dbEndpoint;
            set { _dbEndpoint = value; OnPropertyChanged("DbEndpoint"); }
        }

        public ISample SelectedSample { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<ISample> Samples
        {
            get => _samples;
            set { _samples = value; OnPropertyChanged("Samples"); }
        }

        private Session() { }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
