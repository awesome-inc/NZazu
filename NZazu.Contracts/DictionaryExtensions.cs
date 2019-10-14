using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

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
                source = new Dictionary<TKey, TValue>();

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

        public static Dictionary<string, string> MergedWith(this IDictionary<string, string> source,
            IDictionary<string, string> toMerge)
        {
            var toBeMerged = source ?? new Dictionary<string, string>();
            var result = toBeMerged.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (toMerge != null)
                foreach (var key in toMerge.Keys)
                    result[key] = toMerge[key];
            return result;
        }

        public static bool MergeWith(this IDictionary<string, string> mergeTo, Dictionary<string, string> toMerge)
        {
            var anyChanges = false;
            foreach (var field in toMerge)
                if (!mergeTo.ContainsKey(field.Key) || mergeTo[field.Key] != field.Value)
                {
                    mergeTo[field.Key] = field.Value;
                    anyChanges = true;
                }

            return anyChanges;
        }

        public static string Get(this IDictionary<string, string> settings, string key, string defaultValue = null)
        {
            settings.TryGetValue(key, out var value);
            return value ?? defaultValue;
        }

        public static T? Get<T>(this Dictionary<string, string> settings, string key)
            where T : struct
        {
            var str = settings.Get(key);

            try
            {
                if (str == null) return null;
                return (T) Convert.ChangeType(str, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                Trace.TraceWarning(
                    $"Setting {key} with value '{str ?? "<null>"}' has the wrong type. A {typeof(T).Name} is expected.");
                return null;
            }
        }

        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
                someObjectType
                    .GetProperty(item.Key)
                    ?.SetValue(someObject, item.Value, null);

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }
    }
}