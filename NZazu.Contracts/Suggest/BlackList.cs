using System;
using NZazu.Contracts.Suggest;

// ReSharper disable CheckNamespace
namespace NSuggest
// ReSharper restore CheckNamespace
{
    public class BlackList<TKey> : IBlackList<TKey> where TKey : IEquatable<TKey>
    {
        private readonly Cache<TKey, int> _cache;

        public BlackList(int maxSize = 8192)
        {
            _cache = new Cache<TKey, int>(maxSize);
        }

        public int Capacity
        {
            get { return _cache.Capacity; }
            set { _cache.Capacity = value; }
        }

        public int Count => _cache.Count;

        public bool Push(TKey key)
        {
            int count;
            if (_cache.TryGetValue(key, out count))
                _cache[key] = count + 1;
            else
                _cache.Add(key, 1);
            return true;
        }

        public bool Pop(TKey key)
        {
            return _cache.Remove(key);
        }

        public bool IsBlackListed(TKey key, int maxFails)
        {
            int count;
            _cache.TryGetValue(key, out count);
            return (count >= maxFails); // NOTE: count = 0, if not contained in the black list.
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public override string ToString()
        {
            return _cache.ToString();
        }
    }
}