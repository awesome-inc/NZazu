using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Extensions
{
    public class KeyValuesStore
    {
        private readonly Dictionary<string, List<string>> _store = new Dictionary<string, List<string>>();

        public IEnumerable<string> GetValues(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (key == string.Empty)
                return _store.SelectMany(x => x.Value);

            if (!_store.ContainsKey(key))
                return Enumerable.Empty<string>();

            return _store[key];
        }

        public void Add(string key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (!_store.ContainsKey(key))
                _store.Add(key, new List<string>());

            var values = _store[key];
            values.Add(value);
        }
    }
}