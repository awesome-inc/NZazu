using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts.Suggest
{
    /// <summary>
    ///     aggregation provider merging different providers into one. this provider
    ///     uses a select many in case many provider handle the same data connection.
    /// </summary>
    public class AggregateProvideSuggestions : IProvideSuggestions
    {
        private readonly IEnumerable<IProvideSuggestions> _provider;

        public AggregateProvideSuggestions(IEnumerable<IProvideSuggestions> provider)
        {
            _provider = provider;
        }

        public IEnumerable<string> For(string prefix, string dataconnection)
        {
            // a SelectMany() does not seen to work. strange.
            var result = new List<string>();
            foreach (var p in _provider)
            {
                var res = p.For(prefix, dataconnection) ?? Enumerable.Empty<string>();
                result.AddRange(res);
            }

            return result.ToArray();
        }
    }
}