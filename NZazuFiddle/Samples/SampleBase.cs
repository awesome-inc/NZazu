using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    public abstract class SampleBase : IHaveSample
    {
        protected SampleBase(int order)
        {
            Order = order;
        }

        public ISample Sample { get; protected set; }
        public int Order { get; }

        protected static IFiddle ToFiddle(FormDefinition formDefinition, FormData formData)
        {
            var events = new EventAggregator();
            return new FiddleViewModel(
                new FormDefinitionViewModel(events,formDefinition), 
                new FormDataViewModel(events, formData),
                new PreviewViewModel(events, formDefinition, formData));
        }
    }
}