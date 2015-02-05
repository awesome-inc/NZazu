using System.Collections.Generic;
using NZazu.Contracts;

namespace Sample.Samples
{
    class PrimitivesSample : IHaveSample
    {
        public INZazuSample Sample { get; private set; }

        public PrimitivesSample()
        {
            Sample = new NZazuSampleViewModel
            {
                Name = "Primitives",
                Description = "",
                FormDefinition = new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "settings",
                            Type = "label",
                            Prompt = "Settings",
                            Description = "You can manage your account here."
                        },
                        new FieldDefinition
                        {
                            Key = "name",
                            Type = "string",
                            Prompt = "Name",
                            Hint = "Enter name",
                            Description = "Your account name. Only alpha-numeric ..."
                        },
                        new FieldDefinition
                        {
                            Key = "isAdmin",
                            Type = "bool",
                            //Prompt = "Is Admin",
                            Hint = "Is Admin",
                            Description = "Check to grant administrator permissions"
                        }
                    }
                },
                FormData = new Dictionary<string, string> {{"name", "John"}, {"isAdmin", "true"}}
            };
        }
    }
}