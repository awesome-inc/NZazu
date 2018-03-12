using System;
using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    public class FormDataViewModel : HaveJsonFor<FormData>, IFormDataViewModel
    {
        private readonly IEventAggregator _events;
        private FormData _data;
        private bool _inHandle;

        public FormDataViewModel(IEventAggregator events, FormData data)
        {
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _events.Subscribe(this);
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public FormData Data
        {
            get { return _data; }
            set
            {
                if (Equals(value, _data)) return;
                _data = value ?? new FormData();
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(Json));
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

        public override FormData Model { get { return Data; } set { Data = value; } }
    }
}