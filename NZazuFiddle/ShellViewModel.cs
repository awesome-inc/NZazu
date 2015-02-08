using System;
using System.Collections.Generic;
using System.Globalization;
using Caliburn.Micro;
using NZazu;
using NZazu.Contracts;
using NZazu.Layout;
using NZazu.Xceed;

namespace NZazuFiddle
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShellViewModel : Screen, IShell
    {
        private FormDefinition _definition = SampleFormDefinition();
        private FormData _data = SampleFormData();
        private INZazuFieldFactory _fieldFactory = new XceedFieldFactory();
        private INZazuLayoutStrategy _layoutStrategy = new GridLayoutStrategy();
        private const string DateFormat = @"yyyy-MM-dd";

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

        public INZazuFieldFactory FieldFactory
        {
            get { return _fieldFactory; }
            private set
            {
                if (Equals(value, _fieldFactory)) return;
                _fieldFactory = value;
                NotifyOfPropertyChange();
            }
        }

        public INZazuLayoutStrategy LayoutStrategy
        {
            get { return _layoutStrategy; }
            private set
            {
                if (Equals(value, _layoutStrategy)) return;
                _layoutStrategy = value;
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
                        Checks = new []
                        {
                            new CheckDefinition { Type = "required" }, 
                            new CheckDefinition { Type="length", Values=new []{"6", "8"} }
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