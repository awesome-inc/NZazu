using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Remove<TKey, TValue>(
            this Dictionary<TKey, TValue> source,
            Func<KeyValuePair<TKey, TValue>, bool> predicate)
            where TValue : class
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            var validItems = source.Where((kvp, i) => !predicate(kvp));
            return validItems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}