using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    class FormDefinitionViewModel : PropertyChangedBase, IFormDefinitionViewModel
    {
        private FormDefinition _formDefinition;

        public FormDefinition FormDefinition
        {
            get { return _formDefinition; }
            set
            {
                if (Equals(value, _formDefinition)) return;
                _formDefinition = value;
                NotifyOfPropertyChange();
            }
        }
    }
}