using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    class MixedGroupLayout : SampleBase
    {
        public MixedGroupLayout() : base(50)
        {
            Sample = new SampleViewModel
            {
                Name = "Layout",
                Description = "An example showing mixed layout types with group fields",
                Fiddle = ToFiddle(new FormDefinition
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
                                        new FieldDefinition { Key = "left.caption", Type = "label", Prompt="Left" },
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
                                        new FieldDefinition { Key = "right.caption", Type = "label", Prompt="Right" },
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
                new Dictionary<string, string>
                {
                    { "left.name", "John" }, 
                    { "right.name", "Jim" }, 
                    { "left.score", "15"},
                    { "right.score", "35"}
                })
            };
        }
    }
}