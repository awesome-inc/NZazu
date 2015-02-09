using System.Collections.Generic;
using NZazu.Contracts;

namespace Sample.Samples
{
    class BehaviorSample : IHaveSample
    {
        public INZazuSample Sample { get; private set; }

        public int Order { get { return 40; } }

        public BehaviorSample()
        {
            Sample = new NZazuSampleViewModel
            {
                Name = "Behavior",
                Description = "",
                FormDefinition = new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "comment",
                            Type = "richtext",
                            Prompt = "Comment",
                            Hint = "",
                            Description = "describe your impression of the weather",
                            Checks = new []
                            {
                                new CheckDefinition { Type = "required" }, 
                                new CheckDefinition { Type="length", Values=new []{"100"} }
                            },
                            Behavior = new BehaviorDefinition { Name = "autourl" },
                        },
                    }
                },
                FormData = new Dictionary<string, string>
                {
                    { "name", "John" }, 
                    { "email", "foobar"},
                    { "isAdmin", "false" }
                }
            };
        }
    }
}