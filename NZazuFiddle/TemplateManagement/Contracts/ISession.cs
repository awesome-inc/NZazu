using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.TemplateManagement.Contracts
{
    interface ISession
    {

        string Endpoint { get; set; }

        ISample SelectedSample { get; set; }

        List<ISample> Samples { get; set; }

    }
}
