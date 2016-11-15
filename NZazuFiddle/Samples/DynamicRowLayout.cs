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
                Name = "Dynamic Rows",
                Description = "Test for a dynamic table with columns defined as fields",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "datatable",
                            Type = "group",
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