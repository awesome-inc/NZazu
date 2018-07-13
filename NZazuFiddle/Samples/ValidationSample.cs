using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    // ReSharper disable once UnusedMember.Global
    internal class ValidationSample : SampleBase
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
                                new CheckDefinition { Type = "required" }, 
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
                        },
                        new FieldDefinition
                        {
                            Key = "startTime",
                            Type = "string",
                            Prompt = "Star time",
                            Hint = "Set start time"
                        },
                        new FieldDefinition
                        {
                            Key = "endTime",
                            Type = "string",
                            Prompt = "End time",
                            Hint = "Set end time",
                            Checks = new []
                            {
                                new CheckDefinition
                                {
                                    Type = "dateTime", Values = new []{ "End time must lie in future compared to start time", ">", "startTime" }
                                }
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "startTimeWithFormats",
                            Type = "string",
                            Prompt = "Another start time",
                            Hint = "Set start time"
                        },
                        new FieldDefinition
                        {
                            Key = "endTimeWithFormats",
                            Type = "string",
                            Prompt = "Another end time",
                            Hint = "Set end time with specific format to compare with anothe start time format",
                            Checks = new []
                            {
                                new CheckDefinition
                                {
                                    Type = "dateTime", Values = new []{ "End time must lie in future compared to start time", ">", "startTimeWithFormats", "HHmm|HHmmss|HH:mm|HH:mm:ss" }
                                }
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