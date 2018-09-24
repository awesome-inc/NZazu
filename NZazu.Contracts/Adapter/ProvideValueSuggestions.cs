using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NZazu.Contracts.Adapter
{
    /// <summary>
    /// provider for native values retrieved from nzazu form definition file.
    /// settings: "dataconnection" -> "v:anton|adam|abraham|anna|annika|astrid"
    /// </summary>
    public class ProvideValueSuggestions : IProvideSuggestions
    {
        public IEnumerable<string> For(string prefix, string dataconnection)
        {
            if (!dataconnection.StartsWith("v:")) return Enumerable.Empty<string>();

            Trace.WriteLine($"trying to get suggestions for {prefix} from {dataconnection}");
            return dataconnection
                .Substring(2)
                .Split('|')
                .Where(x => x.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}