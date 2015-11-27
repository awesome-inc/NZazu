using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using NZazuFiddle.Samples;

namespace NZazuFiddle
{
    internal sealed class ShellViewModel : Screen,IShell
    {
        private readonly BindableCollection<ISample> _samples = new BindableCollection<ISample>();
        private ISample _selectedSample;

        public ShellViewModel(IEnumerable<IHaveSample> samples = null)
        {
            DisplayName = "NZazuFiddle";
            if (samples != null) 
                Samples = samples.OrderBy(s => s.Order).Select(s => s.Sample);
        }

        public IEnumerable<ISample> Samples
        {
            get { return _samples; }
            set
            {
                if (Equals(value, _samples)) return;
                _samples.Clear();
                if (value != null) _samples.AddRange(value);
                SelectedSample = _samples.FirstOrDefault();
            }
        }

        public ISample SelectedSample
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