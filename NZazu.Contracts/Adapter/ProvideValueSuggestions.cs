using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NZazu.Contracts.Adapter
{
    public class ProvideValueSuggestions : IProvideSuggestions
    {
        public IEnumerable<string> For(string prefix, string dataconnection)
        {
            Trace.WriteLine($"tring to get suggestions for {prefix} from {dataconnection}");

            if (!dataconnection.StartsWith("v:")) return Enumerable.Empty<string>();

            return dataconnection.Substring(2).Split('|').Where(x => x.StartsWith(prefix));
        }
    }
}