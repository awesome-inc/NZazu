using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using NZazu;
using NZazu.Contracts;

namespace Sample
{
    public class NZazuSampleViewModel : Screen, INZazuSample
    {
        private readonly Dictionary<string, string> _formData = new Dictionary<string, string>();

        public string Name { get; set; }
        public string Description { get; set; }
        public FormDefinition FormDefinition { get; set; }

        public IDictionary<string, string> FormData
        {
            get { return _formData; }
            set
            {
                if (_formData.SequenceEqual(value ?? new Dictionary<string, string>())) return;
                _formData.Clear();
                if (value != null) value.ToList().ForEach(kvp => _formData.Add(kvp.Key, kvp.Value));
                NotifyOfPropertyChange();
            }
        }

        public void ApplyChanges()
        {
            var view = GetView() as NZazuSampleView;
            if (view == null) return;

            var isValid = view.NZazuView.IsValid();
            if (!isValid)
            {
                MessageBox.Show("this view contains invalid data");
                return;
            }

            view.NZazuView.ApplyChanges();
            FormData = view.NZazuView.FormData;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}