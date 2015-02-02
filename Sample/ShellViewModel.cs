using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using NZazu.Contracts;

namespace Sample
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShellViewModel : PropertyChangedBase, IShell
    {
        private readonly BindableCollection<INZazuSample> _samples = new BindableCollection<INZazuSample>();
        private INZazuSample _selectedSample;

        public ShellViewModel()
        {
            Samples = new[]
            {
                new NZazuSample
                {
                    Name = "Primitives",
                    Description = "",
                    FormDefinition = new FormDefinition
                    {
                        Fields = new[]
                        {
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
                    }
                }
            };
        }

        public IEnumerable<INZazuSample> Samples
        {
            get { return _samples; }
            set
            {
                _samples.Clear();
                if (value != null)
                    _samples.AddRange(value);
                NotifyOfPropertyChange();
                SelectedSample = _samples.FirstOrDefault();
            }
        }

        public INZazuSample SelectedSample
        {
            get { return _selectedSample; }
            set
            {
                if (Equals(value, _selectedSample)) return;
                _selectedSample = value;
                NotifyOfPropertyChange();
            }
        }
    }
}