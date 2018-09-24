using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NZazu.Contracts.Adapter
{
    /// <summary>
    /// provider for native values retrieved from a file containing a entry per row string array.
    /// settings: "dataconnection" -> "f:[file path]"
    /// </summary>
    public class ProvideFileSuggestions : IProvideSuggestions
    {
        readonly IDictionary<string, IEnumerable<string>> _cache = new Dictionary<string, IEnumerable<string>>();

        public IEnumerable<string> For(string prefix, string dataconnection)
        {
            if (!dataconnection.StartsWith("f:")) return Enumerable.Empty<string>();

            Trace.WriteLine($"tring to get suggestions for {prefix} from {dataconnection}");
            var file = dataconnection.Substring(2);
            var values = GetValues(file);
            return values.Where(x => x.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
        }

        private IEnumerable<string> GetValues(string file)
        {
            // already read, so just return values without re-read
            if (_cache.ContainsKey(file))
                return _cache[file];

            // we dont cache non existing file
            var fi = new FileInfo(file);
            if (!fi.Exists)
            {
                Trace.WriteLine($"cannot get calues from file '{file}' because file does not exist. returning empty array. nothing added to cache");
                return Enumerable.Empty<string>();
            }

            // lets get the data, cache it and return it
            var fileContent = File.ReadAllLines(file);
            _cache.Add(file, fileContent);
            return fileContent;
        }
    }
}