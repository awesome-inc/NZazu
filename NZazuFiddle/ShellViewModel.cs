using System;
using System.Collections.Generic;
using System.Globalization;
using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShellViewModel : Screen, IShell
    {
        private FormDefinition _definition = SampleFormDefinition();
        private FormData _data = SampleFormData();
        private const string DateFormat = @"ddHHmm\Z MMM yy";

        public FormDefinition Definition
        {
            get { return _definition; }
            set
            {
                if (Equals(value, _definition)) return;
                _definition = value;
                NotifyOfPropertyChange();
            }
        }

        public FormData Data
        {
            get { return _data; }
            set
            {
                if (Equals(value, _data)) return;
                _data = value;
                NotifyOfPropertyChange();
            }
        }

        private static FormDefinition SampleFormDefinition()
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
                        },
                        new FieldDefinition
                        {
                            Key = "birthday",
                            Type = "date",
                            Format = DateFormat,
                            Prompt = "Date of Birth",
                            Hint = "Enter your date of birth",
                            Description = "Your birthday",
                        },
                        new FieldDefinition
                        {
                            Key = "weight",
                            Type = "double",
                            Prompt = "Weight",
                            Format = "#.00",
                            Hint = "Enter your body weight",
                            Description = "Your body weight in kg",
                        },

                        new FieldDefinition
                        {
                            Key = "isAdmin",
                            Type = "bool",
                            Hint = "Is Admin",
                            Description = "Check to grant administrator permissions"
                        }
                    }
            };
        }

        private static FormData SampleFormData()
        {
            return new Dictionary<string, string>
            {
                {"name", "John"}, 
                {"birthday", new DateTime(1980, 1,1).ToString(DateFormat)}, 
                {"weight", 82.4d.ToString(CultureInfo.InvariantCulture)}, 
                {"isAdmin", "true"}
            };
        }
    }
}