using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace NZazu.Contracts.Suggest
{
    /// <summary>
    ///     A generic thread-safe lru-cache.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys to use for lookup.</typeparam>
    /// <typeparam name="TValue">The type of the values to keep in the cache.</typeparam>
    public sealed class Cache<TKey, TValue> : ICache<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private const int DefaultInitialCapacity = 1024;
        private const int DefaultMaxSize = 4096;
        private const float DefaultKeepfree = 0f;

        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private readonly IDictionary<TKey, Node> _hashMap;

        private int _capacity;
        private Node _head;
        private float _keepFree = DefaultKeepfree; // keep some [0,1]-fraction of the cache free
        private Node _tail;

        /// <summary>
        ///     Number of cache hits, since the last update.
        /// </summary>
        public int CacheHits;

        /// <summary>
        ///     Number of cache misses since the last update.
        /// </summary>
        public int CacheMisses;

        /// <summary>
        ///     Initializes a new instance of Cache&lt;K, V&gt; with the default maximum size (1024).
        /// </summary>
        public Cache()
            : this(DefaultMaxSize, DefaultInitialCapacity)
        {
        }

        /// <summary>
        ///     Initializes a new instance of Cache&lt;K, V&gt;.
        /// </summary>
        /// <param name="maxSize">
        ///     The maximum number of elements allowed in the cache. The minimum
        ///     allowed value is 1.
        /// </param>
        public Cache(int maxSize)
            : this(maxSize, maxSize < DefaultInitialCapacity ? maxSize : DefaultInitialCapacity)
        {
        }

        /// <summary>
        ///     Initializes a new instance of Cache&lt;K, V&gt;.
        /// </summary>
        /// <param name="maxSize">
        ///     The maximum number of elements allowed in the cache. The minimum
        ///     allowed value is 1.
        /// </param>
        /// <param name="initialCapacity">The initial capacity.</param>
        public Cache(int maxSize, int initialCapacity)
        {
            Capacity = maxSize;
            _hashMap = new Dictionary<TKey, Node>(initialCapacity);
        }

        /// <summary>
        ///     Gets the <i>cache state</i> in values between 0 and 100, which
        ///     is a measure of how well the cache currently performs in hiding latency.
        ///     When using the cache in combination with transmitting schemes this
        ///     property may be used to indicate the percentage of data arrived.
        /// </summary>
        /// <value>The state.</value>
        public int State => Math.Max(0, 100 - 100 * CacheMisses / Math.Max(1, CacheHits));

        /// <summary>
        ///     Gets or sets the keep free percentage value, i.e. a value between 0 and 0.5
        ///     specifiying the relative space in the cache to keep free. This is used
        ///     to avoid stuttering on alternating Add, Remove when the cache capacity
        ///     is reached.
        /// </summary>
        /// <remarks>Reasonable values lie between 0.1 and 0.4</remarks>
        /// <value>The keep free percentage.</value>
        public float KeepFree
        {
            get => _keepFree;
            set
            {
                if (value < 0f || value > 0.5f)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "Invalid keep free fraction specified (negative or greater than 0.5).");
                _keepFree = value;
            }
        }

        /// <summary>
        ///     Gets the desired count, i.e. maximum size minus the keep-free fraction, which is
        ///     employed to minimize tail removing frequency.
        /// </summary>
        /// <value>The desired count.</value>
        public int DesiredCount => (int) ((1.0f - KeepFree) * (Capacity - 1));

        /// <summary>
        ///     Occurs when a node is to be removed.
        /// </summary>
        public event CacheEventHandler<TKey, TValue> NodeRemoved;

        /// <summary>
        ///     Gets or sets the maximum size of the cache. Minimum size is 1.
        /// </summary>
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "Invalid cache size specified (zero or negative).");
                _capacity = value;
            }
        }

        /// <summary>
        ///     Gets the number of elements contained in the cache.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if the specified key does not exist.</exception>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>value to speicified key</returns>
        public TValue this[TKey key]
        {
            get
            {
                _cacheLock.EnterUpgradeableReadLock();
                try
                {
                    Node node;
                    if (_hashMap.TryGetValue(key, out node))
                    {
                        if (!node.Key.Equals(_head.Key))
                        {
                            _cacheLock.EnterWriteLock();
                            try
                            {
                                PromoteOrInsertNode(node);
                            }
                            finally
                            {
                                _cacheLock.ExitWriteLock();
                            }
                        }

                        return node.Value;
                    }
                }
                finally
                {
                    _cacheLock.ExitUpgradeableReadLock();
                }

                return default(TValue);
            }
            set
            {
                _cacheLock.EnterUpgradeableReadLock();
                try
                {
                    Node node;
                    if (_hashMap.TryGetValue(key, out node))
                    {
                        _cacheLock.EnterWriteLock();
                        try
                        {
                            PromoteOrInsertNode(node);
                            node.Value = value;
                        }
                        finally
                        {
                            _cacheLock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    _cacheLock.ExitUpgradeableReadLock();
                }
            }
        }

        /// <summary>
        ///     Removes the item at the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>troe on success, false otherwise.</returns>
        public bool Remove(TKey key)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                Node node;
                if (_hashMap.TryGetValue(key, out node))
                {
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        InternalRemove(node);
                        return true;
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }

            return false;
        }

        /// <summary>
        ///     Removes all items matching a given key predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of items removed.</returns>
        public int RemoveKey(Predicate<TKey> predicate)
        {
            var itemsRemoved = Count;

            _cacheLock.EnterWriteLock();
            try
            {
                var nodes = _hashMap.Values.ToArray();
                foreach (var node in nodes)
                    if (predicate(node.Key))
                        InternalRemove(node);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }

            itemsRemoved -= Count;
            return itemsRemoved;
        }

        /// <summary>
        ///     Removes all items matching a given value predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of items removed.</returns>
        public int RemoveValues(Predicate<TValue> predicate)
        {
            var itemsRemoved = Count;

            _cacheLock.EnterWriteLock();
            try
            {
                var nodes = _hashMap.Values.ToArray();
                foreach (var node in nodes)
                    if (predicate(node.Value))
                        InternalRemove(node);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }

            itemsRemoved -= Count;
            return itemsRemoved;
        }

        /// <summary>
        ///     Flushes the cache.
        /// </summary>
        public void Clear()
        {
            _cacheLock.EnterWriteLock();
            try
            {
                var nodes = _hashMap.Values.ToArray();
                foreach (var node in nodes)
                    OnRemoveNode(node);

                _head = _tail = null;
                Count = 0;
                _hashMap.Clear();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Adds an element with the provided key and value to the cache.
        /// </summary>
        /// <param name="key">The object to use as the key of the element.</param>
        /// <param name="value">The object to use as the element.</param>
        public void Add(TKey key, TValue value)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (!_hashMap.ContainsKey(key))
                {
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        if (Count >= _capacity)
                            TidyUp();
                        var node = new Node(key, value);
                        PromoteOrInsertNode(node);
                        _hashMap.Add(key, node);
                        Count++;
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key,
        /// </param>
        /// if the key is found; otherwise the default value of for the type of the value
        /// parameter. This parameter is passed uninitialized.
        /// <returns>true on success, false otherwise.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                Node node;
                if (_hashMap.TryGetValue(key, out node))
                {
                    // need to change doubly linked lru list?
                    if (!node.Key.Equals(_head.Key))
                    {
                        _cacheLock.EnterWriteLock();
                        try
                        {
                            PromoteOrInsertNode(node);
                        }
                        finally
                        {
                            _cacheLock.ExitWriteLock();
                        }
                    }

                    value = node.Value;
                    CacheHits++;
                    return true;
                }
                else
                {
                    value = default(TValue);
                    CacheMisses++;
                    return false;
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        ///     Determines whether the Cache contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if containing an item with specified key, false otherwise.</returns>
        public bool ContainsKey(TKey key)
        {
            _cacheLock.EnterReadLock();
            try
            {
                if (_hashMap.ContainsKey(key))
                {
                    CacheHits++;
                    return true;
                }
                else
                {
                    CacheMisses++;
                    return false;
                }
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Called when a nodes is removed.
        /// </summary>
        /// <param name="node">The node.</param>
        private void OnRemoveNode(Node node)
        {
            if (NodeRemoved != null)
                NodeRemoved(this, new Tuple<TKey, TValue>(node.Key, node.Value));
        }

        private void InternalRemove(Node node)
        {
            _hashMap.Remove(node.Key);

            // connect previous and next
            if (node.Previous == null) // head
            {
                _head = node.Next;
                if (_head != null)
                    _head.Previous = null;
            }
            else
            {
                node.Previous.Next = node.Next;
            }

            if (node.Next == null) // tail
            {
                _tail = node.Previous;
                if (_tail != null)
                    _tail.Next = null;
            }
            else
            {
                node.Next.Previous = node.Previous;
            }

            Count--;

            OnRemoveNode(node);
        }

        private void RemoveTail()
        {
            if (Count <= 0) return;

            Debug.Assert(_tail != null);
            _hashMap.Remove(_tail.Key);

            OnRemoveNode(_tail);
            Debug.Assert(_tail.Previous != null);

            _tail = _tail.Previous;
            _tail.Next = null;

            Count--;
        }

        /// <summary>
        ///     Tidies up by removing tail until reaching the desired count.
        /// </summary>
        private void TidyUp()
        {
            var desiredCount = DesiredCount;
            while (Count >= desiredCount)
                RemoveTail();
        }

        /// <summary>
        ///     Promotes or inserts node by moving it to the head.
        /// </summary>
        /// <param name="node">The node.</param>
        private void PromoteOrInsertNode(Node node)
        {
            Debug.Assert(_cacheLock.IsWriteLockHeld);

            if (node == null) throw new ArgumentNullException(nameof(node), "Internal Cache Error.");

            // first entry
            if (_head == null)
            {
                _head = _tail = node;
            }
            else
            {
                Debug.Assert(_head != null);
                if (!_head.Key.Equals(node.Key)) // requires IEquatable<K>
                {
                    // node is tail, move tail to previous
                    Debug.Assert(_tail != null);
                    if (_tail.Key.Equals(node.Key))
                    {
                        Debug.Assert(node.Previous != null);
                        _tail = node.Previous;
                        _tail.Next = null;
                    }
                    else
                    {
                        // remove node from list
                        if (node.Previous != null) // cannot be head
                            node.Previous.Next = node.Next;
                        if (node.Next != null)
                            node.Next.Previous = node.Previous;
                    }

                    // insert before head
                    node.Next = _head;
                    _head.Previous = node;

                    // make node new head
                    _head = node;
                    _head.Previous = null;
                }
            }
        }

        /// <summary>
        ///     Resets the hit/miss statistics.
        /// </summary>
        public void ResetStatistics()
        {
            CacheHits = CacheMisses = 0;
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            //return base.ToString();
            return $"Cur/Max = {Count}/{Capacity}, Hit/Miss = {CacheHits}/{CacheMisses} ({State}%)";
        }

        /// <summary>
        ///     Cache node class, doubly linked list item.
        /// </summary>
        private class Node
        {
            /// <summary>
            ///     The key.
            /// </summary>
            public TKey Key;

            /// <summary>
            ///     Reference to the next node in the doubly linked list.
            /// </summary>
            public Node Next;

            /// <summary>
            ///     Reference to the previous node in the doubly linked list.
            /// </summary>
            public Node Previous;

            /// <summary>
            ///     The value.
            /// </summary>
            public TValue Value;

            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}