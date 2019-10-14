using System;
using System.Collections.Generic;

namespace NZazu.Contracts.Suggest
{
    public interface IHaveKeyedValues<TValue>
    {
        /// <summary>
        ///     Gets all items with keys matching the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns />
        IEnumerable<Tuple<string, TValue>> ItemsMatching(string prefix);

        IEnumerable<string> KeysMatching(string prefix);
    }
}