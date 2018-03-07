using Caliburn.Micro;
using NZazu.Contracts;
using NZazuFiddle.TemplateManagement;
using NZazuFiddle.TemplateManagement.Contracts;
using System;
using System.Windows;

namespace NZazuFiddle
{
    class AddTemplateViewModel : Screen, IAddTemplateViewModel
    {
        private string _sampleId;
        private readonly ISession _session;
        private readonly IEventAggregator _globalEvents;

        public string SampleId { get => _sampleId; set => _sampleId = value; }

        public bool CanAddSample => true;

        public AddTemplateViewModel(ISession session, IEventAggregator globalEvents)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _globalEvents = globalEvents ?? throw new ArgumentNullException(nameof(globalEvents));
        }

        public void AddSample()
        {
            var newTemplateSample = new TemplateSample(_sampleId, _sampleId, new FormDefinition(), new FormData());
            _session.AddSampleAsUniqueItem(newTemplateSample.Sample);

        }
    }
}
