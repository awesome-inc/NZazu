using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle
{
    interface INewFileViewModel
    {
        string SampleId { get; set; }
        bool CanAcceptButton { get; set; }
        void AcceptButton();
        void CancelButton();
    }
}
