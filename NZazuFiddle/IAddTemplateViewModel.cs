using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle
{
    interface IAddTemplateViewModel : IScreen
    {

        string SampleId { get; set; }

        bool CanAddSample { get; }

        void AddSample();

    }
}
