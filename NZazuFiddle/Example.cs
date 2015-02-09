using System;
using System.Collections.Generic;
using System.Globalization;
using NZazu.Contracts;

namespace NZazuFiddle
{
    public class Example
    {
        private const string DateFormat = @"yyyy-MM-dd";

        public static FormDefinition FormDefinition 
        {
            get
            {
                return new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "caption",
                            Type = "label",
                            Prompt = "Settings"
                        },
                        new FieldDefinition
                        {
                            Key = "name",
                            Type = "string",
                            Prompt = "Name",
                            Hint = "Enter name",
                            Description = "Your account name",
                            Checks = new[]
                            {
                                new CheckDefinition {Type = "required"},
                                new CheckDefinition {Type = "length", Values = new[] {"6", "8"}}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "birthday",
                            Type = "date",
                            Settings = new Dictionary<string, string> {{"Format", DateFormat}},
                            Prompt = "Date of Birth",
                            Hint = "Enter your date of birth",
                            Description = "Your birthday",
                        },
                        new FieldDefinition
                        {
                            Key = "ranking",
                            Type = "int",
                            Prompt = "Ranking",
                            Hint = "Enter your ranking (0-100)",
                            Description = "Your ranking",
                            Checks = new[]
                            {
                                new CheckDefinition {Type = "range", Values = new[] {"0", "100"}}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "weight",
                            Type = "double",
                            Prompt = "Weight",
                            Settings = new Dictionary<string, string> {{"Format", "#.00"}},
                            Hint = "Enter your body weight",
                            Description = "Your body weight in kg",
                        },

                        new FieldDefinition
                        {
                            Key = "isAdmin",
                            Type = "bool",
                            Hint = "Is Admin",
                            Description = "Check to grant administrator permissions",
                            Checks = new[]
                            {
                                new CheckDefinition
                                {
                                    Type = "regex",
                                    Values = new[] {"Must be Checked or Unchecked", "True", "False"}
                                }
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "notes",
                            Type = "richtext",
                            Prompt = "Notes",
                            Description = "Notes for the current issue",
                            Settings = new Dictionary<string, string> {{"Height", 80.ToString(CultureInfo.InvariantCulture)}}
                        }
                    }
                };
            }
        }

        public static FormData FormData
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"name", "John"},
                    {"birthday", new DateTime(1980, 1, 1).ToString(DateFormat)},
                    {"ranking", "50"},
                    {"weight", "82.4"},
                    {"isAdmin", "true"}
                };
            }
        }
    }
}