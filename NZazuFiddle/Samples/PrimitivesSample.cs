using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    class PrimitivesSample : SampleBase
    {
        public PrimitivesSample() : base(10)
        {
            Sample = new SampleViewModel
            {
                Name = "Primitives",
                Description = "",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "caption",
                            Type = "label",
                            Description = "A fancy caption!"
                        },
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
                        },
                        new FieldDefinition
                        {
                            Key = "birthday",
                            Type = "date",
                            Prompt = "Birthday",
                            Hint = "type your birthday",
                        },
                        new FieldDefinition
                        {
                            Key = "weight",
                            Type = "double",
                            Prompt = "Weight in kg",
                            Hint = "type your weight with 2 digits aber comma",
                        },
                    }
                },
                new Dictionary<string, string>
                {
                    { "name", "John" }, 
                    { "isAdmin", "true" }
                })
            };
        }
    }
}