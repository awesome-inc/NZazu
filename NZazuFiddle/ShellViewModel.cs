using System.Collections.Generic;
using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShellViewModel : Screen, IShell
    {
        private FormDefinition _definition = SampleFormDefinition();
        private FormData _data = SampleFormData();

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
                        }
                    }
            };
        }

        private static FormData SampleFormData()
        {
            return new Dictionary<string, string> {{"name", "John"}, {"isAdmin", "true"}};
        }
    }
}