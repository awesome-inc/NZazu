using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Sample.Samples;

namespace Sample
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShellViewModel : Screen, IShell
    {
        private readonly BindableCollection<INZazuSample> _samples = new BindableCollection<INZazuSample>();
        private INZazuSample _selectedSample;

        public ShellViewModel(IEnumerable<IHaveSample> samples)
        {
            Samples = samples.OrderBy(s => s.Order).Select(s => s.Sample).ToArray();
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
                if (_selectedSample != null) _selectedSample.ApplyChanges();
                _selectedSample = value;
                NotifyOfPropertyChange();
            }
        }
    }
}