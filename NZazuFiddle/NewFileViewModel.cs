using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle
{
    class NewFileViewModel : Screen
    {

        public string SampleId { get; set; }

        public bool IsCancelled { get; set; }

        public bool CanAcceptButton
        {
            get { return true; /* add logic here */ }
        }

        public void AcceptButton()
        {
            IsCancelled = false;
            TryClose(true);
        }

        public bool CanCancelButton
        {
            get { return true; }
        }

        public void CancelButton()
        {
            IsCancelled = true;
            TryClose(false);
        }
    }
}
