using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NZazu.Contracts.Suggest
{
    /// <summary>
    ///     provider for native values retrieved from a file containing a entry per row string array.
    ///     settings: "dataConnection" -> "f:[file path]"
    /// </summary>
    public class ProvideFileSuggestions : IProvideSuggestions
    {
        private const string ConnectionPrefix = "file://";

        private readonly IDictionary<string, IEnumerable<string>>
            _cache = new Dictionary<string, IEnumerable<string>>();

        private readonly IFileSystem _filesystem;

        public ProvideFileSuggestions(IFileSystem filesystem = null)
        {
            _filesystem = filesystem ?? new FileSystem();
        }

        public IEnumerable<string> For(string prefix, string dataConnection)
        {
            if (!dataConnection.StartsWith(ConnectionPrefix)) return Enumerable.Empty<string>();
            if (string.IsNullOrWhiteSpace(prefix)) return Enumerable.Empty<string>();

            Trace.WriteLine($"string to get suggestions for {prefix} from {dataConnection}");
            var file = dataConnection.Substring(ConnectionPrefix.Length);
            var values = GetValues(file);
            return values.Where(x => x.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
        }

        private IEnumerable<string> GetValues(string file)
        {
            // already read, so just return values without re-read
            if (_cache.ContainsKey(file))
                return _cache[file];

            // we don't cache non existing file
            if (!_filesystem.FileExists(file))
            {
                Trace.WriteLine(
                    $"cannot get values from file '{file}' because file does not exist. returning empty array. nothing added to cache");
                return Enumerable.Empty<string>();
            }

            // lets get the data, cache it and return it
            var fileContent = _filesystem.ReadAllLines(file);
            _cache.Add(file, fileContent);
            return fileContent;
        }
    }
}