using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NZazu.Contracts;
using NZazu.FieldBehavior;
using NZazuFiddle.Samples;

namespace NZazuFiddle
{
    class SampleTemplate : SampleBase
    {

        public SampleTemplate(): base(40) { }

        public SampleTemplate(string name, FormDefinition sampleFormDefinition, FormData sampleFormData) : base(40)
        {
            Sample = new SampleViewModel
            {
                Name = name,
                Fiddle = ToFiddle(sampleFormDefinition, sampleFormData)

            };
        }

    }
}
