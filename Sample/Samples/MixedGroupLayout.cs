using System.Collections.Generic;
using NZazu.Contracts;
using NZazu.Xceed;

namespace Sample.Samples
{
    class MixedGroupLayout : IHaveSample
    {
        public INZazuSample Sample { get; private set; }
        public int Order { get { return 50; } }

        public MixedGroupLayout()
        {
            Sample = new NZazuSampleViewModel
            {
                FieldFactory = new XceedFieldFactory(),
                Name = "Layout",
                Description = "An example showing mixed layout types with group fields",
                FormDefinition = new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key="caption", Type="label", Prompt = "Note"
                        },
                        new FieldDefinition
                        {
                            Key="timestamp", Type="date", Prompt = "Date"
                        },
                        new FieldDefinition
                        {
                            Key = "stack",
                            Layout = "stack",
                            Type = "group",
                            Fields = new[]
                            {
                                new FieldDefinition
                                {
                                    Key = "left",
                                    Type = "group",
                                    Settings = new Dictionary<string, string>{{"Width", "150"}},
                                    Fields = new []
                                    {
                                        new FieldDefinition { Key = "left.name", Type = "string", Prompt="Name" },
                                        new FieldDefinition { Key = "left.score", Type = "double", Prompt="Score" }
                                    }
                                },
                                new FieldDefinition
                                {
                                    Key = "right",
                                    Type = "group",
                                    Settings = new Dictionary<string, string>{{"Width", "150"}},
                                    Fields = new []
                                    {
                                        new FieldDefinition { Key = "right.name", Type = "string" },
                                        new FieldDefinition { Key = "right.score", Type = "double" }
                                    }
                                }
                            }  
                        },
                        new FieldDefinition
                        {
                            Key="comment", Type="richtext", Prompt = "Comment"
                        }
                    }
                },
                FormData = new Dictionary<string, string>
                {
                    { "left.name", "John" }, 
                    { "right.name", "Jim" }, 
                    { "left.score", "15"},
                    { "right.score", "35"}
                }
            };
        }
    }
}