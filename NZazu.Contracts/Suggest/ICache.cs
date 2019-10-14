using System;
using NEdifis.Attributes;

namespace NZazu.Contracts.Suggest
{
    [ExcludeFromConventions("nothing to test here")]
    public delegate void CacheEventHandler<TKey, TValue>(object sender, Tuple<TKey, TValue> e);

    /// <summary>
    ///     An interface for a generic thread-safe lru-cache.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public interface ICache<TKey, TValue>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Gets or sets the maximum cache size.
        /// </summary>
        /// <value />
        int Capacity { get; set; }

        /// <summary>
        ///     Gets the current number of items in the cache.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets or sets the value for the specified key index.
        /// </summary>
        TValue this[TKey key] { get; set; }

        /// <summary>
        ///     Occurs when a node has been automatically removed from the cache due to size limitations.
        /// </summary>
        event CacheEventHandler<TKey, TValue> NodeRemoved;

        /// <summary>
        ///     Adds the specified key and value to the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Add(TKey key, TValue value);

        /// <summary>
        ///     Determines whether the cache contains a value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns />
        bool ContainsKey(TKey key);

        /// <summary>
        ///     Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Removes the value with the specified key from the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns />
        bool Remove(TKey key);

        /// <summary>
        ///     Removes all items in the cache matching the specified key predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of items removed.</returns>
        int RemoveKey(Predicate<TKey> predicate);

        /// <summary>
        ///     Removes all items in the cache matching the specified value predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of items removed.</returns>
        int RemoveValues(Predicate<TValue> predicate);

        /// <summary>
        ///     Tries getting the value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns />
        bool TryGetValue(TKey key, out TValue value);
    }
}