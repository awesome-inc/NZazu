using System;
using Caliburn.Micro;
using NZazu;
using NZazu.Contracts;
using NZazu.Xceed;

namespace NZazuFiddle
{
    public class PreviewViewModel : Screen, IPreviewViewModel
    {
        private readonly IEventAggregator _events;
        private FormDefinition _definition;
        private FormData _data;
        private bool _inHandle;
        private INZazuWpfFieldFactory _fieldFactory;
        private bool _isReadOnly;

        public PreviewViewModel(IEventAggregator events, 
            FormDefinition definition, FormData data, 
            INZazuWpfFieldFactory fieldFactory = null)
        {
            if (events == null) throw new ArgumentNullException("events");
            if (definition == null) throw new ArgumentNullException("definition");
            if (data == null) throw new ArgumentNullException("data");
            _events = events;
            _events.Subscribe(this);
            _definition = definition;
            _data = data;
            _fieldFactory = fieldFactory ?? new XceedFieldFactory();
        }

        public FormDefinition Definition
        {
            get { return _definition; }
            set
            {
                if (Equals(value, _definition)) return;
                _definition = value;
                NotifyOfPropertyChange();
            }
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

        public INZazuWpfFieldFactory FieldFactory
        {
            get { return _fieldFactory; }
            private set
            {
                if (Equals(value, _fieldFactory)) return;
                _fieldFactory = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                if (value.Equals(_isReadOnly)) return;
                _isReadOnly = value;
                NotifyOfPropertyChange();
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var view = GetView() as PreviewView;
            if (view == null) return;

            view.View.TrySetFocusOn(Definition.FocusOn);
        }

        public void Handle(FormDefinition definition)
        {
            Definition = definition;
        }

        public void Handle(FormData data)
        {
            if (_inHandle) return;
            _inHandle = true;
            try { Data = data; }
            finally { _inHandle = false; }
        }
    }
}