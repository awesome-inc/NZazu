using Caliburn.Micro;
using System.Linq;

namespace NZazuFiddle
{
    class NewFileViewModel : Screen
    {

        private string _sampleId = "";

        public string SampleId
        {
            get => _sampleId;
            set
            {
                _sampleId = value.Trim();
                NotifyOfPropertyChange(nameof(SampleId));
                NotifyOfPropertyChange(nameof(CanAcceptButton));
            }
        }

        public bool IsCancelled { get; set; }

        public bool CanAcceptButton
        {
            get => !string.IsNullOrWhiteSpace(SampleId) || SampleId.ToCharArray().Contains(' ');
        }

        public void AcceptButton()
        {
            IsCancelled = false;
            TryClose(true);
        }

        public void CancelButton()
        {
            IsCancelled = true;
            TryClose(false);
        }
 
    }
}
