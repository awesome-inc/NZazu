using NZazu.Contracts;
using System.Collections.Generic;

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
                                new CheckDefinition {
                                    Type ="length",
                                    Settings = new Dictionary<string ,string>{ {"Min", "6" } }
                                }
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
                                new CheckDefinition
                                {
                                    Type = "regex",
                                    Settings = new Dictionary<string ,string>{{"Hint", "Must be a valid e-mail address" },{"RegEx", @"^.+@.+\..+$" } }
                                }
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
                                new CheckDefinition
                                {
                                    Type = "regex",
                                    Settings = new Dictionary<string ,string>{{"Hint", "Must be Checked or Unchecked" },{"RegEx", "True|False" } }
                                }
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
                                    Type = "dateTime",
                                    Settings = new Dictionary<string, string>
                                    {
                                        {"Hint", "End time must lie in future compared to start time" },
                                        {"CompareOperator",">"},
                                        {"FieldToCompareWith","startTime"},
                                    }
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
                            Hint = "Set end time with specific format to compare with another start time format",
                            Checks = new []
                            {
                                new CheckDefinition
                                {
                                    Type = "dateTime",
                                    Settings = new Dictionary<string, string>
                                    {
                                        {"Hint", "End time must lie in future compared to start time" },
                                        {"CompareOperator",">"},
                                        {"FieldToCompareWith","startTimeWithFormats"},
                                        {"SpecificDateTimeFormats", "HHmm|HHmmss|HH:mm|HH:mm:ss" }
                                    }
                                }
                            }
                        },
                        new FieldDefinition
                        {
                            Type = "datatable",
                            Key = "intraTableComparison",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "startTimeTable",
                                    Type = "string",
                                    Prompt = "Start time (table)"
                                },
                                new FieldDefinition
                                {
                                    Key = "stopTimeTable",
                                    Type = "string",
                                    Prompt = "Stop time (table)",
                                    Checks = new []
                                    {
                                        new CheckDefinition
                                        {
                                            Type = "dateTime",
                                            Settings = new Dictionary<string, string>
                                            {
                                                {"Hint", "End time must lie in future compared to start time" },
                                                {"CompareOperator",">"},
                                                {"FieldToCompareWith","startTime"},
                                                {"TableToCompareWith","intraTableComparison"},
                                                {"SpecificDateTimeFormats", "HHmm|HHmmss|HH:mm|HH:mm:ss" }
                                            }
                                        }
                                    }
                                },
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