using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    class BehaviorSample : SampleBase
    {
        public BehaviorSample() : base(40)
        {
            Sample = new SampleViewModel
            {
                Name = "Behavior",
                Fiddle = ToFiddle(
                new FormDefinition
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
                        }
                    }
                },
                new Dictionary<string, string>
                {
                    { "comment", "John" }, 
                })
            };
        }
    }
}