using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.TemplateManagement.Contracts
{
    internal interface ITemplateDbClient
    {
        string Endpoint { get; set; }

        Task<List<ISample>> GetData();

        Task<string> UpdateData(ISample sample);

        string DeleteData(string id);

        string CreateData(ISample sample);

    }
}
