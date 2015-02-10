using System;
using Caliburn.Micro;
using NZazu;
using NZazu.Contracts;
using NZazu.LayoutStrategy;
using NZazu.Xceed;

namespace NZazuFiddle
{
    public class PreviewViewModel : Screen, IPreviewViewModel
    {
        private readonly IEventAggregator _events;
        private FormDefinition _definition;
        private FormData _data;
        private INZazuWpfFieldFactory _fieldFactory;
        private INZazuWpfLayoutStrategy _layoutStrategy;
        private bool _inHandle;

        public PreviewViewModel(IEventAggregator events, FormDefinition definition, FormData data,
            INZazuWpfFieldFactory fieldFactory = null,
            INZazuWpfLayoutStrategy layoutStrategy = null)
        {
            if (events == null) throw new ArgumentNullException("events");
            if (definition == null) throw new ArgumentNullException("definition");
            if (data == null) throw new ArgumentNullException("data");
            _events = events;
            _events.Subscribe(this);
            _definition = definition;
            _data = data;
            _fieldFactory = fieldFactory ?? new XceedFieldFactory();
            _layoutStrategy = layoutStrategy ?? new GridLayout();
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
            set
            {
                if (Equals(value, _fieldFactory)) return;
                _fieldFactory = value;
                NotifyOfPropertyChange();
            }
        }

        public INZazuWpfLayoutStrategy LayoutStrategy
        {
            get { return _layoutStrategy; }
            set
            {
                if (Equals(value, _layoutStrategy)) return;
                _layoutStrategy = value;
                NotifyOfPropertyChange();
            }
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