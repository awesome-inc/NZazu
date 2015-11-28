using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    internal class MixedGroupLayout : SampleBase
    {
        public MixedGroupLayout()
            : base(50)
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
                            Key="caption", Type="label", Prompt = "Note", 
                            Description = "Mixed Layout Sample with customized tabbing: horizontal between left, right"
                        },
                        new FieldDefinition
                        {
                            Key="timestamp", Type="date", Prompt = "Date", 
                            Settings = new Dictionary<string, string>{{"TabIndex","0"}}
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
                                        new FieldDefinition
                                        {
                                            Key = "left.name", Type = "string", Prompt="Name",
                                            Settings = new Dictionary<string, string>{{"TabIndex","1"}}
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "left.score", Type = "double", Prompt="Score",
                                            Settings = new Dictionary<string, string>{{"TabIndex","3"}}
                                        }
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
                                        new FieldDefinition
                                        {
                                            Key = "right.name", Type = "string",
                                            Settings = new Dictionary<string, string>{{"TabIndex","2"}}
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "right.score", Type = "double",
                                            Settings = new Dictionary<string, string>{{"TabIndex","4"}}
                                        }
                                    }
                                }
                            }  
                        },
                        new FieldDefinition
                        {
                            Key="comment", Type="richtext", Prompt = "Comment",
                            Settings = new Dictionary<string, string>{{"TabIndex","5"}}
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