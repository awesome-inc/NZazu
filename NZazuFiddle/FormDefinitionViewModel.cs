using System;
using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    public class FormDefinitionViewModel : Screen, IFormDefinitionViewModel
    {
        private readonly IEventAggregator _events;
        private FormDefinition _definition;
        private bool _inHandle;

        public FormDefinitionViewModel(IEventAggregator events, 
            FormDefinition definition = null)
        {
            if (events == null) throw new ArgumentNullException("events");
            _events = events;
            _events.Subscribe(this);
            _definition = definition ?? Example.FormDefinition;
        }

        public FormDefinition Definition
        {
            get { return _definition; }
            set
            {
                if (Equals(value, _definition)) return;
                _definition = value;
                NotifyOfPropertyChange();
                if (!_inHandle) _events.PublishOnUIThread(_definition);
            }
        }

        public void Handle(FormDefinition definition)
        {
            if (_inHandle) return;
            _inHandle = true;
            try { Definition = definition; }
            finally { _inHandle = false; }
        }
    }
}