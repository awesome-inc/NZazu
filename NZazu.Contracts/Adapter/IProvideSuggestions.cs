using System.Collections.Generic;

namespace NZazu.Contracts.Adapter
{
    public interface IProvideSuggestions
    {
        IEnumerable<string> For(string prefix, string dataconnection);
    }
}