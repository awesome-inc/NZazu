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
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var validItems = source.Where((kvp, i) => !predicate(kvp));
            return validItems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static bool Equivalent(this Dictionary<string, string> dict, Dictionary<string, string> dict2)
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

        public static Dictionary<string, string> MergedWith(this Dictionary<string, string> source, Dictionary<string, string> toMerge)
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

        public static bool MergeWith(this Dictionary<string, string> mergeTo, Dictionary<string, string> toMerge)
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