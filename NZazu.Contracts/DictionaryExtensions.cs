using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts
{
    public static class DictionaryExtensions
    {
        public static IDictionary<TKey, TValue> AddOrReplace<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            TKey key, TValue value)
            where TValue : class
        {
            if (source == null)
                source = new Dictionary<TKey,TValue>();

            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);

            return source;
        }

        public static IDictionary<TKey, TValue> Remove<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            Func<KeyValuePair<TKey, TValue>, bool> predicate)
            where TValue : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var validItems = source.Where((kvp, i) => !predicate(kvp));
            return validItems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static bool Equivalent(this IDictionary<string, string> dict, IDictionary<string, string> dict2)
        {
            // cf.: http://www.dotnetperls.com/dictionary-equals
            // Test for equality.
            var equal = false;
            if (dict.Count == dict2.Count) // Require equal count.
            {
                equal = true;
                foreach (var pair in dict)
                {
                    string value;
                    if (dict2.TryGetValue(pair.Key, out value))
                    {
                        // Require value be equal.
                        if (value != pair.Value)
                        {
                            equal = false;
                            break;
                        }
                    }
                    else
                    {
                        // Require key be present.
                        equal = false;
                        break;
                    }
                }
            }
            return equal;
        }

        public static IDictionary<string, string> MergedWith(this IDictionary<string, string> source, IDictionary<string, string> toMerge)
        {
            var toBeMerged = source ?? new Dictionary<string, string>();
            var result = toBeMerged.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (toMerge != null)
            {
                foreach (var key in toMerge.Keys)
                    result[key] = toMerge[key];
            }
            return result;
        }

        public static bool MergeWith(this IDictionary<string, string> mergeTo, IDictionary<string, string> toMerge)
        {
            var anyChanges = false;
            foreach (var field in toMerge)
            {
                if (!mergeTo.ContainsKey(field.Key) || mergeTo[field.Key] != field.Value)
                {
                    mergeTo[field.Key] = field.Value;
                    anyChanges = true;
                }
            }
            return anyChanges;
        }

    }
}