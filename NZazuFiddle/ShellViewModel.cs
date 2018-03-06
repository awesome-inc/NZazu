using Caliburn.Micro;
using Microsoft.Win32;
using NZazuFiddle.Samples;
using NZazuFiddle.TemplateManagement;
using NZazuFiddle.TemplateManagement.Contracts;
using NZazuFiddle.TemplateManagement.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xceed.Wpf.Toolkit;

namespace NZazuFiddle
{
    internal sealed class ShellViewModel : Screen, IShell
    {
        private readonly BindableCollection<ISample> _samples;
        private ISample _selectedSample;

        public ShellViewModel(IEndpointViewModel endpointViewModel, IFileMenuViewModel fileMenuViewModel, ISession session)
        {
            DisplayName = "TACON Template Editor";

            _samples = session.Samples;
            EndpointViewModel = endpointViewModel;
            FileMenuViewModel = fileMenuViewModel;
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

        public IEndpointViewModel EndpointViewModel { get; }

        public IFileMenuViewModel FileMenuViewModel { get; }

    }
}