using System;
using Caliburn.Micro;
using NZazu;
using NZazu.Contracts;
using NZazu.Contracts.Adapter;
using NZazu.Contracts.Suggest;
using NZazu.JsonSerializer;
using NZazu.JsonSerializer.RestSuggestor;
using NZazu.Xceed;

namespace NZazuFiddle
{
    public class PreviewViewModel : Screen, IPreviewViewModel
    {
        private readonly IEventAggregator _events;
        private FormData _data;
        private FormDefinition _definition;
        private INZazuWpfFieldFactory _fieldFactory;
        private bool _inHandle;
        private bool _isReadOnly;

        public PreviewViewModel(IEventAggregator events,
            FormDefinition definition, FormData data,
            INZazuWpfFieldFactory fieldFactory = null)
        {
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _events.Subscribe(this);
            _definition = definition ?? throw new ArgumentNullException(nameof(definition));
            _data = data ?? throw new ArgumentNullException(nameof(data));

            _fieldFactory = fieldFactory ?? new XceedFieldFactory();
            _fieldFactory.Use<INZazuTableDataSerializer>(new NZazuTableDataJsonSerializer());
            _fieldFactory.Use<ISupportGeoLocationBox>(new SupportGeoLocationBox());

            var dbSuggestor =
                new SuggestionsProxy(
                        new ElasticSearchSuggestions(
                            connectionPrefix: "http://127.0.0.1:9200")) // instead of localhost. this changes the server
                    {
                        BlackListSize = 10,
                        CacheSize = 3000,
                        MaxFailures = int.MaxValue
                    };

            // lets do some stuff with the suggestor
            _fieldFactory.Use<IProvideSuggestions>(new AggregateProvideSuggestions(new IProvideSuggestions[]
            {
                new ProvideValueSuggestions(),
                new ProvideFileSuggestions(),
                dbSuggestor
            }));
        }

        public FormDefinition Definition
        {
            get => _definition;
            set
            {
                if (Equals(value, _definition)) return;
                _definition = value;
                NotifyOfPropertyChange();
            }
        }

        public FormData Data
        {
            get => _data;
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
            get => _fieldFactory;
            set
            {
                if (Equals(value, _fieldFactory)) return;
                _fieldFactory = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (value.Equals(_isReadOnly)) return;
                _isReadOnly = value;
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
            try
            {
                Data = data;
            }
            finally
            {
                _inHandle = false;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (Data.Values == null || !Data.Values.ContainsKey(NZazuView.FocusOnFieldName)) return;

            var view = GetView() as PreviewView;
            view?.View.TrySetFocusOn();
        }
    }
}