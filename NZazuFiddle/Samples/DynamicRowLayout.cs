using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    internal class DynamicRowLayout : SampleBase
    {
        public DynamicRowLayout() : base(-10)
        {
            Sample = new SampleViewModel
            {
                Name = "Radio Transmission",
                Description = "Test for a dynamic table with columns defined as fields",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {


                        new FieldDefinition
                        {
                            Key="group_activity",
                            Type="group",
                            Prompt = "Activity",
                            Layout = "stack",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "group_activity_stack_left",
                                    Type = "group",
                                    Settings = new Dictionary<string, string>() { {"Width","300" } },
                                    Fields = new []
                                    {
                                        new FieldDefinition
                                        {
                                            Key = "dtg",
                                            Type = "date",
                                            Prompt = "DTG",
                                            Description = "Date of the incident",
                                        },
                                    }
                                },
                                new FieldDefinition
                                {
                                    Key = "group_activity_stack_right",
                                    Type = "group",
                                    Fields = new []
                                    {
                                        new FieldDefinition
                                        {
                                            Key = "frequency",
                                            Type = "double",
                                            Prompt = "Frequency",
                                        },
                                    }
                                }
                            }
                        },

                        new FieldDefinition
                        {
                            Key = "datatable",
                            Type = "datatable",
                            Prompt = "Enter your data",
                            Description = "Awesome table",
                            Fields = new[]
                            {
                                new FieldDefinition
                                {
                                    Key = "name",
                                    Type = "string",
                                    Prompt = "Your Name",
                                    Description = "The name your parents gave you"
                                },
                                new FieldDefinition
                                {
                                    Key = "lastname",
                                    Type = "string",
                                    Prompt = "Lastname",
                                    Description = "You know, your family name"
                                },
                                new FieldDefinition
                                {
                                    Key = "birthday",
                                    Type = "date",
                                    Prompt = "Birthday",
                                    Settings = new Dictionary<string, string> {{"TabIndex", "0"}}
                                }
                            }
                        }
                    }
                }, new FormData())
            };
        }
    }
}