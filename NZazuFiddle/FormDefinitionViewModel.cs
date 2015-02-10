using System;
using System.Diagnostics;
using Caliburn.Micro;
using NZazu.Contracts;
using Action = System.Action;

namespace NZazuFiddle
{
    public class FormDefinitionViewModel : Screen, IFormDefinitionViewModel
    {
        private readonly IEventAggregator _events;
        private FormDefinition _definition;
        private bool _inHandle;

        public FormDefinitionViewModel(IEventAggregator events, 
            FormDefinition definition)
        {
            if (events == null) throw new ArgumentNullException("events");
            if (definition == null) throw new ArgumentNullException("definition");
            _events = events;
            _events.Subscribe(this);
            _definition = definition;
        }

        public FormDefinition Definition
        {
            get { return _definition; }
            set
            {
                if (Equals(value, _definition)) return;
                _definition = value;
                NotifyOfPropertyChange();
                if (value == null || _inHandle) return;
                Safe(() => _events.PublishOnUIThread(_definition), "Could not set form definition");
            }
        }

        public void Handle(FormDefinition definition)
        {
            if (_inHandle) return;
            _inHandle = true;
            try { Definition = definition; }
            finally { _inHandle = false; }
        }

        static void Safe(Action action, string couldNot)
        {
            try { action(); }
            catch (Exception ex) { Trace.TraceWarning("{0}: {1}", couldNot, ex.Message); }
        }

    }
}