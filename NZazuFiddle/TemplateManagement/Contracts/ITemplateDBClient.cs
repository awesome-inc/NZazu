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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        /// <exception cref="SaveSamplesException">Thrown when</exception>
        void UpdateData(ISample sample);

        void DeleteData(string id);

        void CreateData(ISample sample);

    }
}
