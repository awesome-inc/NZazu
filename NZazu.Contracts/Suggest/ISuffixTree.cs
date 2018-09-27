using System;
using System.Collections.Generic;

namespace NZazu.Contracts.Suggest
{
    public interface ISuffixTree : IHaveKeyStrings
    {
        /// <summary>
        /// Determines whether this instance contains any keys with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix string.</param>
        /// <returns />
        bool Any(string prefix);

        /// <summary>
        /// Removes the specified key string.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True, if the string could be removed; otherwise false</returns>
        bool TryRemove(string key);

        /// <summary>
        /// Adds the specified key string.
        /// </summary>
        /// <param name="key">The string to add.</param>
        /// <returns>True, if the string could be added; otherwise false</returns>
        bool TryAdd(string key);

        /// <summary>
        /// Adds a range of key strings.
        /// </summary>
        /// <param name="keys">The keys to add.</param>
        /// <param name="sort">If true sort keys ascending before adding.</param>
        void AddRange(IEnumerable<string> keys, bool sort = true);
    }

    public interface ISuffixTree<TValue> : IHaveKeyedValues<TValue>
    {
        /// <summary>
        /// Determines whether this instance contains any keys with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix string.</param>
        /// <returns />
        bool Any(string prefix);

        /// <summary>
        /// Removes the specified key string.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True, if the string could be removed; otherwise false</returns>
        bool TryRemove(string key);

        /// <summary>
        /// Adds the specified key and value.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value corresponding to the key.</param>
        /// <returns>
        /// True, if the key could be added; otherwise false
        /// </returns>
        bool TryAdd(string key, TValue value);

        /// <summary>
        /// Adds a range of items to the tree.
        /// </summary>
        /// <param name="items">The items to add.</param>
        /// <param name="sort">If true sort items ascendig by key before adding.</param>
        void AddRange(IEnumerable<Tuple<string, TValue>> items, bool sort = true);
    }
}