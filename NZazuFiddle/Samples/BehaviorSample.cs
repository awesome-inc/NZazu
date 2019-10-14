using System.Collections.Generic;
using NZazu.Contracts;
using NZazu.FieldBehavior;

namespace NZazuFiddle.Samples
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class BehaviorSample : SampleBase
    {
        public BehaviorSample()
            : base(40)
        {
            Sample = new SampleViewModel
            {
                Name = "Behavior",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "headercomment",
                            Prompt = "",
                            Type = "label",
                            Description = "in the dialog below you can enter a text and open a url by hitting STRG+Return"
                        },
                        new FieldDefinition
                        {
                            Key = "comment",
                            Type = "string",
                            Prompt = "Comment",
                            Hint = "",
                            Settings = new Dictionary<string, string>{{"Height", "100"}},
                            Description = "describe your impression of the weather",
                            Checks = new []
                            {
                                new CheckDefinition { Type = "required" }
                            },
#pragma warning disable 618
                            Behaviors = new[] { new BehaviorDefinition { Name = "OpenUrlOnStringEnter" }},
#pragma warning restore 618
                        },
                        new FieldDefinition
                        {
                            Key ="group",
                            Type = "group",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "nested.comment", Type ="string",
#pragma warning disable 618
                                    Behaviors = new[]{new BehaviorDefinition { Name = "OpenUrlOnStringEnter" }}
#pragma warning restore 618
                                }
                            }
                        },
                        // Check multiple behaviours
                        new FieldDefinition
                        {
                            Key = "input1",
                            Type = "string",
                            Prompt = "Input 1",
                            Hint = "Input something ...\r\nIn this textbox you can test 2 behaviours ('OpenUrlOnStringEnter', 'SetBorder')\r\n--> set on 'Behaviors' collection prop",
                            Settings = new Dictionary<string, string>{{"Height", "100"}},
                            Description = "non sense",
                            Behaviors = new List<BehaviorDefinition>() {new BehaviorDefinition { Name = "OpenUrlOnStringEnter" }, new BehaviorDefinition { Name = "SetBorder" }},
                        },
                        // Check single behaviour and multiple behaviours coexistence
                        new FieldDefinition
                        {
                            Key = "input2",
                            Type = "string",
                            Prompt = "Input 2",
                            Hint = "Input something ...\r\nIn this textbox you can test 2 behaviours ('OpenUrlOnStringEnter', 'SetBorder')\r\n--> set one on 'Behavior' prop and the other one on the 'Behaviors' collection prop",
                            Settings = new Dictionary<string, string>{{"Height", "100"}},
                            Description = "non sense",
                            Behaviors = new List<BehaviorDefinition>()
                            {
                                new BehaviorDefinition { Name = "SetBorder" },
                                new BehaviorDefinition { Name = "OpenUrlOnStringEnter" }
                            },
                        }
                    }
                },
                new Dictionary<string, string>
                {
                    { "comment", "type in a url like http://google.de and open it with Ctrl+Enter" },
                    { "nested.comment", "type in a url like http://google.de and open it with Ctrl+Enter" },
                })
            };
        }
    }
}