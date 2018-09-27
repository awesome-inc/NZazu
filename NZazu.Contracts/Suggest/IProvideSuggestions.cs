using System.Collections.Generic;

namespace NZazu.Contracts.Suggest
{
    public interface IProvideSuggestions
    {
        IEnumerable<string> For(string prefix, string dataconnection);
    }
}