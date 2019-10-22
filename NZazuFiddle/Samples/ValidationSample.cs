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
                                Key = "errorOverview",
                                Type = "error",
                                Prompt = "Validate Now!"
                            },
                        new FieldDefinition
                        {
                            Key = "name",
                            Type = "string",
                            Prompt = "Name",
                            Hint = "Enter name",
                            Description =
                                    "Validation: required field (required) and at least 6 characters (length)",
                            Checks = new[]
                                {
                                    new CheckDefinition {Type = "required"},
                                    new CheckDefinition
                                    {
                                        Type = "length",
                                        Settings = new Dictionary<string, string> {{"Min", "6"}}
                                    }
                                }
                        },
                        new FieldDefinition
                        {
                            Key = "email",
                            Type = "string",
                            Prompt = "Email",
                            Hint = "Enter valid e-mail address",
                            Description = "Validation: regular expression (regex) [^.+@.+\\..+$]",
                            Checks = new[]
                                {
                                    new CheckDefinition
                                    {
                                        Type = "regex",
                                        Settings = new Dictionary<string, string>
                                            {{"Hint", "Must be a valid e-mail address"}, {"RegEx", @"^.+@.+\..+$"}}
                                    }
                                }
                        },
                        new FieldDefinition
                        {
                            Key = "isAdmin",
                            Type = "bool",
                            Hint = "Is Admin",
                            Description = "Validation: regular expression (regex) [True|False]",
                            Checks = new[]
                                {
                                    new CheckDefinition {Type = "required"},
                                    new CheckDefinition
                                    {
                                        Type = "regex",
                                        Settings = new Dictionary<string, string>
                                            {{"Hint", "Must be Checked or Unchecked"}, {"RegEx", "True|False"}}
                                    }
                                },
                            Settings = new Dictionary<string, string>
                                {
                                    {"IsThreeState", "True"}
                                }
                        },
                        new FieldDefinition
                        {
                            Key = "int",
                            Type = "int",
                            Prompt = "int",
                            Hint = "Enter a number",
                            Description = "Validation: Value must be between 10 and 100 (range)",
                            Checks = new[]
                                {
                                    new CheckDefinition
                                    {
                                        Type = "range",
                                        Settings = new Dictionary<string, string>
                                        {
                                            {"Min", "10"},
                                            {"Max", "100"}
                                        }
                                    }
                                }
                        },
                        new FieldDefinition
                        {
                            Key = "startTime",
                            Type = "string",
                            Prompt = "Start time",
                            Hint = "Set start time"
                        },
                        new FieldDefinition
                        {
                            Key = "endTime",
                            Type = "string",
                            Prompt = "End time",
                            Hint = "Set end time",
                            Checks = new[]
                                {
                                    new CheckDefinition
                                    {
                                        Type = "datetime",
                                        Settings = new Dictionary<string, string>
                                        {
                                            {"Hint", "End time must lie in future compared to start time"},
                                            {"CompareOperator", ">"},
                                            {"FieldToCompareWith", "startTime"}
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
                            Description = "Validation: Value must be between 10 and 100 (range)",
                            Checks = new[]
                                {
                                    new CheckDefinition
                                    {
                                        Type = "datetime",
                                        Settings = new Dictionary<string, string>
                                        {
                                            {"Hint", "End time must lie in future compared to start time"},
                                            {"CompareOperator", ">"},
                                            {"FieldToCompareWith", "startTimeWithFormats"},
                                            {"SpecificDateTimeFormats", "HHmm|HHmmss|HH:mm|HH:mm:ss"}
                                        }
                                    }
                                }
                        },
                        new FieldDefinition
                        {
                            Type = "datatable",
                            Key = "interTableComparison",
                            Fields = new[]
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
                                        Checks = new[]
                                        {
                                            new CheckDefinition
                                            {
                                                Type = "datetime",
                                                Settings = new Dictionary<string, string>
                                                {
                                                    {"Hint", "End time must lie in future compared to start time"},
                                                    {"CompareOperator", ">"},
                                                    {"FieldToCompareWith", "startTime"},
                                                    {"TableToCompareWith", "interTableComparison"},
                                                    {"SpecificDateTimeFormats", "HHmm|HHmmss|HH:mm|HH:mm:ss"}
                                                }
                                            }
                                        }
                                    }
                                }
                        }
                    }
                },
                    new Dictionary<string, string>
                    {
                        {"name", "John"},
                        {"email", "foobar"},
                        {"isAdmin", "false"}
                    })
            };
        }
    }
}