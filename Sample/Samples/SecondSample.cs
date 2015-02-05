using NZazu.Contracts;

namespace Sample.Samples
{
    class SecondSample : IHaveSample
    {
        public INZazuSample Sample { get; private set; }

        public SecondSample()
        {
            Sample = new NZazuSampleViewModel
            {
                Name = "Second",
                Description = "A 2nd sample",
                FormDefinition = new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "isAdmin",
                            Type = "bool",
                            //Prompt = "Is Admin",
                            Hint = "Is Admin",
                            Description = "Check to grant administrator permissions"
                        }
                    }
                }
            };
        }
    }
}