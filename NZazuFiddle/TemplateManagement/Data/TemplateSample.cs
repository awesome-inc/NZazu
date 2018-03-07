using Caliburn.Micro;
using NZazu.Contracts;
using NZazuFiddle.Samples;
using NZazuFiddle.TemplateManagement.Contracts;

namespace NZazuFiddle.TemplateManagement
{
    internal class TemplateSample : SampleBase
    {

        public TemplateSample(): base(0) { }

        public TemplateSample(string id, string name, FormDefinition sampleFormDefinition, FormData sampleFormData, ETemplateStatus status = ETemplateStatus.Initial) : base(0)
        {
            // handles local events between FormData, FormDefinition and Preview.
           var fiddleRelatedEvents = new EventAggregator();
            Sample = new SampleViewModel
            (
                id,
                name,
                ToFiddle(sampleFormDefinition, sampleFormData, fiddleRelatedEvents),
                fiddleRelatedEvents,
                status
            );
        }

    }
}
