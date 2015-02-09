using System.Windows;
using Caliburn.Micro;
using NZazu;
using NZazu.Contracts;
using NZazu.Extensions;

namespace Sample
{
    public class NZazuSampleViewModel : Screen, INZazuSample
    {
        private FormData _formData = new FormData();

        public string Name { get; set; }
        public string Description { get; set; }
        public FormDefinition FormDefinition { get; set; }

        public FormData FormData
        {
            get { return _formData; }
            set
            {
                if (Equals(_formData, value)) return;
                _formData = value;
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