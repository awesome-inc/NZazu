using Caliburn.Micro;
using NZazu.Contracts;
using NZazuFiddle.Samples;

namespace NZazuFiddle.TemplateManagement
{
    internal class TemplateSample : SampleBase
    {

        public TemplateSample(): base(0) { }

        public TemplateSample(string id, string name, FormDefinition sampleFormDefinition, FormData sampleFormData, IEventAggregator globalEvents = null) : base(0)
        {
            // handles local events between FormData, FormDefinition and Preview.
            // Additonally informations could be handled in SampleViewModel (e.g. to inform global shell about status information).
           var fiddleRelatedEvents = new EventAggregator();
            Sample = new SampleViewModel
            (
                id,
                name,
                ToFiddle(sampleFormDefinition, sampleFormData, fiddleRelatedEvents),
                fiddleRelatedEvents,
                globalEvents
            );
        }

    }
}
