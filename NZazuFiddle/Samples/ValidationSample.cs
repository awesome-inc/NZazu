using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    class ValidationSample : SampleBase
    {
        public ValidationSample() : base(30)
        {
            Sample = new SampleViewModel
            {
                Name = "Validation",
                Description = "",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "name",
                            Type = "string",
                            Prompt = "Name",
                            Hint = "Enter name",
                            Description = "Your account name. At least 6 characters (Required)...",
                            Checks = new []
                            {
                                new CheckDefinition { Type = "required" }, 
                                new CheckDefinition { Type="length", Values=new []{"6"} }
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "email",
                            Type = "string",
                            Prompt = "Email",
                            Hint = "Enter valid e-mail address",
                            Description = "Your e-mail address",
                            Checks = new []
                            {
                                new CheckDefinition { Type = "required" }, 
                                new CheckDefinition { Type = "regex", Values= new []{"Must be a valid e-mail address", @"^.+@.+\..+$"}}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "isAdmin",
                            Type = "bool",
                            //Prompt = "Is Admin",
                            Hint = "Is Admin",
                            Description = "Check to grant administrator permissions",
                            Checks = new []
                            {
                                new CheckDefinition { Type = "regex", Values= new []{"Must be Checked or Unchecked", "True", "False"}}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "birthday",
                            Type = "date",
                            Prompt = "Birthday",
                            Hint = "type your birthday",
                            Checks = new []
                            {
                                new CheckDefinition { Type = "required" }
                            }
                        }
                    }
                },
                new Dictionary<string, string>
                {
                    { "name", "John" }, 
                    { "email", "foobar"},
                    { "isAdmin", "false" }
                })
            };
        }
    }
}