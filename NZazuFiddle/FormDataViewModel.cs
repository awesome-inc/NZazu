using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    class FormDataViewModel : PropertyChangedBase, IFormDataViewModel
    {
        private FormData _formData;

        public FormData FormData
        {
            get { return _formData; }
            set
            {
                if (Equals(value, _formData)) return;
                _formData = value;
                NotifyOfPropertyChange();
            }
        }
    }
}