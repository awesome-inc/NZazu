using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts.Suggest
{
    /// <summary>
    /// An implementation of the Ternary Search Tree.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is an implementation of the <b>Ternary Search Tree</b>
    /// data structure proposed by J. L. Bentley and R. Sedgewick in their 
    /// paper: Fast algorithms for sorting and searching strings
    /// in Proceedings of the Eighth Annual ACM-SIAM Symposium on Discrete Algorithms, 
    /// New Orleans Louisiana, January 5-7, 1997. 
    /// </para>
    /// <para>
    /// The tree acts as a symbol table: The items must be strings. 
    /// It is generally faster to find a symbol than the <see cref="Hashtable"/> or
    /// <see cref="SortedList"/> classes. It can also perform more complex searches
    /// such as near-neighbor search.
    /// </para>
    /// <para>
    /// Please read the paper to get some insight on the stucture used below.
    /// </para>	
    /// </remarks>
    public class TernarySearchTree<TValue> : ISuffixTree<TValue>
    {
        #region Nested Classes

        private class Node
        {
            public readonly Node Parent;
            public readonly char SplitChar;
            public Node HighChild;
            public bool IsKey;
            public string Key;
            public Node LowChild, MiddleChild;
            public TValue Value;

            public Node(Node parent, char splitChar)
            {
                Parent = parent;
                SplitChar = splitChar;
            }

            public bool HasChildren => (LowChild != null || MiddleChild != null || HighChild != null);

            public bool IsLowChild => (Parent != null && ReferenceEquals(Parent.LowChild, this));

            public bool IsHighChild => (Parent != null && ReferenceEquals(Parent.HighChild, this));

            private bool IsMiddleChild => (Parent != null && ReferenceEquals(Parent.MiddleChild, this));

            public override string ToString()
            {
                char c;
                if (IsMiddleChild)
                    c = 'M';
                else if (IsLowChild)
                    c = 'L';
                else if (IsHighChild)
                    c = 'H';
                else
                    c = 'R'; // must be root

                return IsKey ? $"{c} {SplitChar} {Key}" : $"{c} {SplitChar}";
            }
        }

        #endregion

        private Node _root;

        public TernarySearchTree()
        {
            Count = 0;
        }

        #region Public Interface

        public int Count { get; private set; }

        public IEnumerable<string> Keys
        {
            get { return Items.Select(item => item.Item1); }
        }

        public IEnumerable<Tuple<string, TValue>> Items
        {
            get
            {
                var items = new List<Tuple<string, TValue>>(Count);
                GetItems(_root, items);
                return items;
            }
        }

        public bool Any(string prefix)
        {
            var node = Find(prefix);
            return (node != null);
        }

        public bool Single(string key)
        {
            var node = Find(key);
            return (node != null && node.IsKey);
        }

        public IEnumerable<Tuple<string, TValue>> ItemsMatching(string prefix)
        {
            var matches = new List<Tuple<string, TValue>>();
            var node = Find(prefix);
            if (node != null)
            {
                if (node.IsKey)
                    matches.Add(new Tuple<string,TValue>(node.Key, node.Value));
                GetItems(node.MiddleChild, matches);
            }
            return matches;
        }

        public bool TryAdd(String key, TValue value)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            // create root node if necessary.		
            if (_root == null)
                _root = new Node(null, key[0]);

            // add key
            var node = _root;
            var i = 0;
            while (i < key.Length)
            {
                var c = key[i];
                if (c < node.SplitChar)
                {
                    if (node.LowChild == null)
                        node.LowChild = new Node(node, c);
                    node = node.LowChild;
                }
                else if (c > node.SplitChar)
                {
                    if (node.HighChild == null)
                        node.HighChild = new Node(node, c);
                    node = node.HighChild;
                }
                else
                {
                    ++i;
                    if (i == key.Length)
                    {
                        if (node.IsKey)
                        {
                            //throw new ArgumentException("key already in dictionary.");
                            return false;
                        }
                        break;
                    }

                    if (node.MiddleChild == null)
                        node.MiddleChild = new Node(node, key[i]);
                    node = node.MiddleChild;
                }
            }

            node.IsKey = true;
            node.Key = key;
            node.Value = value;
            Count++;

            return true;
        }

        /// <summary>
        /// Adds a range of items using a recursive partioning around the median element strategy
        /// which is an fair approximation to keeping the tree balanced. 
        /// See the paper "Fast Algorithms for Sorting and Searching Keys" of Bentley and Sedgewick,
        /// e.g. at http://www.cs.princeton.edu/~rs/strings/
        /// </summary>
        /// <param name="items">The items to add.</param>
        /// <param name="sort">if set to <c>true</c> input values will be sorted in ascending key order.</param>
        public void AddRange(IEnumerable<Tuple<string, TValue>> items, bool sort = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            var itms = items.ToArray(); // avoid possible multiple enumeration!
            var keys = itms.Select(item => item.Item1).ToArray();
            if (keys.Length <= 0) return;
            var values = itms.Select(k => k.Item2).ToArray();
            if (sort)
                Array.Sort(keys, values);
            AddBalanced(keys, values, 0, keys.Length);
        }

        public bool TryRemove(String key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Invalid key string specified (null or empty).");

            var node = Find(key);
            if (node == null)
                return false;

            if (node.IsKey)
                Count--;

            // destroy key (!)
            node.IsKey = false;
            node.Key = null;

            // remove all single linked parent nodes up the hierarchy
            while (!node.IsKey && !node.HasChildren && (node.Parent != null))
            {
                // remove node from parent link
                if (node.IsLowChild)
                    node.Parent.LowChild = null;
                else if (node.IsHighChild)
                    node.Parent.HighChild = null;
                else
                    node.Parent.MiddleChild = null;

                // continue up the hierarchy
                node = node.Parent;
            }

            // destroy single linked root (!)
            if (!node.IsKey && !node.HasChildren && ReferenceEquals(node, _root))
                _root = null;

            return true;
        }

        public void Clear()
        {
            Count = 0;
            _root = null;
        }

        public IEnumerable<Tuple<string, TValue>> ItemsLike(string pattern, char wildChar = '*')
        {
            if (String.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(nameof(pattern));
            var matches = new List<Tuple<string, TValue>>();
            PartialMatchSearch(_root, pattern, 0, wildChar, matches);
            return matches;
        }

        public IEnumerable<Tuple<string, TValue>> ItemsNearBy(string key, int distance)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (distance < 0)
                throw new ArgumentOutOfRangeException(nameof(distance));
            var matches = new List<Tuple<string, TValue>>();
            NearNeighborsSearch(_root, key, 0, distance, matches);
            return matches;
        }

        public IEnumerable<string> KeysMatching(string prefix)
        {
            return ItemsMatching(prefix).Select(item => item.Item1);
        }

        public IEnumerable<string> KeysNearBy(string pattern, int distance)
        {
            return ItemsNearBy(pattern, distance).Select(item => item.Item1);
        }

        public IEnumerable<string> KeysLike(string pattern, char wildChar = '*')
        {
            return ItemsLike(pattern, wildChar).Select(item => item.Item1);
        }

        #endregion

        #region Methods

        private static void GetItems(Node node, ICollection<Tuple<string, TValue>> items)
        {
            if (node == null) return;
            GetItems(node.LowChild, items);
            if (node.IsKey)
                items.Add(new Tuple<string, TValue>(node.Key, node.Value));
            else
                GetItems(node.MiddleChild, items);
            GetItems(node.HighChild, items);
        }

        private void AddBalanced(string[] keys, IList<TValue> values, int left, int right)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (left >= right) return;
            var pivot = (left + right) / 2;
            TryAdd(keys[pivot], values[pivot]);
            AddBalanced(keys, values, left, pivot);
            AddBalanced(keys, values, pivot + 1, right);
        }

        /// <summary>
        /// Finds the tree node matching the specified prefix.
        /// </summary>
        /// <returns>The tree node matching the specified prefix, null if not found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is null or empty.</exception>
        private Node Find(String prefix)
        {
            if (String.IsNullOrEmpty(prefix))
                throw new ArgumentNullException(nameof(prefix));

            var node = _root;
            var index = 0;
            while (index < prefix.Length && node != null)
            {
                var c = prefix[index];

                if (c < node.SplitChar)
                    node = node.LowChild;
                else if (c > node.SplitChar)
                    node = node.HighChild;
                else // c == node.SplitChar
                {
                    if (index + 1 == prefix.Length) // found!
                        return node;
                    ++index;
                    node = node.MiddleChild;
                }
            }
            return node;
        }

        private static void PartialMatchSearch(Node node, string key, int index,
            char wildChar, ICollection<Tuple<string, TValue>> matches)
        {
            if (node == null)
                return;

            var c = key[index];
            if (c == wildChar || c < node.SplitChar)
                PartialMatchSearch(node.LowChild, key, index, wildChar, matches);

            if (c == wildChar || c == node.SplitChar)
            {
                if (index < key.Length - 1)
                    PartialMatchSearch(node.MiddleChild, key, index + 1, wildChar, matches);
                else if (node.IsKey)
                    matches.Add(new Tuple<string, TValue>(node.Key, node.Value));
            }

            if (c == wildChar || c > node.SplitChar)
                PartialMatchSearch(node.HighChild, key, index, wildChar, matches);
        }

        private static void NearNeighborsSearch(Node node, string key, int index,
            int dist, ICollection<Tuple<string, TValue>> matches)
        {
            if (node == null || dist < 0)
                return;

            var c = key[index];
            // low child
            if (dist > 0 || c < node.SplitChar)
                NearNeighborsSearch(node.LowChild, key, index, dist, matches);

            // middle child
            if (node.IsKey)
            {
                if (key.Length <= dist + index)
                    matches.Add(new Tuple<string, TValue>(node.Key, node.Value));
            }
            else
            {
                var localIndex = index;
                if (localIndex != key.Length - 1)
                    ++localIndex;
                var localDist = dist;
                if (c != node.SplitChar)
                    --localDist;

                NearNeighborsSearch(node.MiddleChild, key, localIndex, localDist, matches);
            }

            // high child
            if (dist > 0 || c > node.SplitChar)
                NearNeighborsSearch(node.HighChild, key, index, dist, matches);
        }

        #endregion
    }
}