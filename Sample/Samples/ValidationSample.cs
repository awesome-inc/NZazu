using System.Collections.Generic;
using System.Text.RegularExpressions;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace Sample.Samples
{
    class ValidationSample : IHaveSample
    {
        public INZazuSample Sample { get; private set; }

        public ValidationSample()
        {
            Sample = new NZazuSampleViewModel
            {
                Name = "Validation",
                Description = "",
                FormDefinition = new FormDefinition
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
                            Checks = new IValueCheck[] { new RequiredCheck(), new StringLengthCheck(6) }
                        },
                        new FieldDefinition
                        {
                            Key = "email",
                            Type = "string",
                            Prompt = "Email",
                            Hint = "Enter valid e-mail address",
                            Description = "Your e-mail address",
                            Checks = new IValueCheck[]
                            {
                                new RequiredCheck(), 
                                new StringRegExCheck("Must be a valid e-mail address", new Regex(@"^.+@.+\..+$", RegexOptions.IgnoreCase))
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "isAdmin",
                            Type = "bool",
                            //Prompt = "Is Admin",
                            Hint = "Is Admin",
                            Description = "Check to grant administrator permissions",
                            Checks = new IValueCheck[]
                            {
                                new StringRegExCheck("Must be Checked or Unchecked", new Regex("True"), new Regex("False"))
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "birthday",
                            Type = "date",
                            Prompt = "Birthday",
                            Hint = "type your birthday",
                            Checks = new IValueCheck[]
                            {
                                new RequiredCheck(), 
                            }
                        },
                    }
                },
                FormData = new Dictionary<string, string>
                {
                    { "name", "John" }, 
                    { "email", "foobar"},
                    { "isAdmin", "false" }
                }
            };
        }
    }
}