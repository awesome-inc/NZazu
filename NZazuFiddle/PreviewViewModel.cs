﻿using System;
using Caliburn.Micro;
using NZazu;
using NZazu.Contracts;
using NZazu.Layout;
using NZazu.Xceed;

namespace NZazuFiddle
{
    public class PreviewViewModel : Screen, IPreviewViewModel
    {
        private readonly IEventAggregator _events;
        private FormDefinition _definition;
        private FormData _data;
        private INZazuFieldFactory _fieldFactory;
        private INZazuLayoutStrategy _layoutStrategy;
        private bool _inHandle;

        public PreviewViewModel(IEventAggregator events,
            FormDefinition definition = null,
            FormData data = null,
            INZazuFieldFactory fieldFactory = null,
            INZazuLayoutStrategy layoutStrategy = null)
        {
            if (events == null) throw new ArgumentNullException("events");
            _events = events;
            _events.Subscribe(this);
            _definition = definition ?? Example.FormDefinition;
            _data = data ?? Example.FormData;
            _fieldFactory = fieldFactory ?? new XceedFieldFactory();
            _layoutStrategy = layoutStrategy ?? new GridLayoutStrategy();
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

        public INZazuFieldFactory FieldFactory
        {
            get { return _fieldFactory; }
            set
            {
                if (Equals(value, _fieldFactory)) return;
                _fieldFactory = value;
                NotifyOfPropertyChange();
            }
        }

        public INZazuLayoutStrategy LayoutStrategy
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