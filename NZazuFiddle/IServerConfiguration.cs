using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle
{
    public interface IServerConfiguration
    {
        string Endpoint { get; set; }

        Task<string> GetDataFromEndpoint(string endpoint);

    }
}
