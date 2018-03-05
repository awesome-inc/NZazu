using NZazu.Contracts;
using NZazuFiddle.Samples;

namespace NZazuFiddle.TemplateManagement
{
    internal class TemplateSample : SampleBase
    {

        public TemplateSample(): base(0) { }

        public TemplateSample(string id, string name, FormDefinition sampleFormDefinition, FormData sampleFormData) : base(0)
        {
            Sample = new SampleViewModel
            {
                Id = id,
                Name = name,
                Fiddle = ToFiddle(sampleFormDefinition, sampleFormData)
            };
        }

    }
}
