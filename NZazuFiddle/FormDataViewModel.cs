using System;
using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    public class FormDataViewModel : Screen, IFormDataViewModel
    {
        private readonly IEventAggregator _events;
        private FormData _data;
        private bool _inHandle;

        public FormDataViewModel(IEventAggregator events, FormData data = null)
        {
            if (events == null) throw new ArgumentNullException("events");
            _events = events;
            _events.Subscribe(this);
            _data = data ?? Example.FormData;
        }

        public FormData Data
        {
            get { return _data; }
            set
            {
                if (Equals(value, _data)) return;
                _data = value;
                NotifyOfPropertyChange();
                if (!_inHandle) _events.PublishOnUIThread(_data);
            }
        }

        public void Handle(FormData formData)
        {
            if (_inHandle) return;
            _inHandle = true;
            try { Data = formData; }
            finally { _inHandle = false; } 
        }
    }
}