using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NZazu.Contracts.Suggest
{
    /// <summary>
    ///     provider for native values retrieved from nzazu form definition file.
    ///     settings: "dataConnection" -> "v:anton|adam|abraham|anna|annika|astrid"
    /// </summary>
    public class ProvideValueSuggestions : IProvideSuggestions
    {
        private const string ConnectionPrefix = "value://";

        public IEnumerable<string> For(string prefix, string dataconnection)
        {
            if (!dataconnection.StartsWith(ConnectionPrefix)) return Enumerable.Empty<string>();

            Trace.WriteLine($"trying to get suggestions for {prefix} from {dataconnection}");
            return dataconnection
                .Substring(ConnectionPrefix.Length)
                .Split('|')
                .Where(x => x.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}