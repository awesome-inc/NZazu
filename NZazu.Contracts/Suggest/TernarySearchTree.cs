using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NEdifis.Attributes;

namespace NZazu.Contracts.Suggest
{
    /// <summary>
    ///     Ternary Search Tree.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class is an implementation of the <b>Ternary Search Tree</b>
    ///         data structure proposed by J. L. Bentley and R. Sedgewick in their
    ///         paper: Fast algorithms for sorting and searching strings
    ///         in Proceedings of the Eighth Annual ACM-SIAM Symposium on Discrete Algorithms,
    ///         New Orleans Louisiana, January 5-7, 1997.
    ///     </para>
    ///     <para>
    ///         The tree acts as a symbol table: The keys must be strings.
    ///         It is generally faster to find a symbol than the <see cref="Hashtable" /> or
    ///         <see cref="SortedList" /> classes. It can also perform more complex searches
    ///         such as near-neighbor search.
    ///     </para>
    ///     <para>
    ///         Please read the paper to get some insight on the structure used below.
    ///     </para>
    /// </remarks>
    [ExcludeFromCodeCoverage]
    [Because("stolen from the internet")]
    [ExcludeFromConventions("stolen from the internet")]
    public class TernarySearchTree
    {
        private Node _root;

        #region Nested Classes

        private class Node
        {
            public readonly Node Parent;
            public readonly char SplitChar;
            public Node HighChild;
            public bool IsKey;
            public string Key;
            public Node LowChild, MiddleChild;

            public Node(Node parent, char splitChar)
            {
                Parent = parent;
                SplitChar = splitChar;
            }

            public bool HasChildren => LowChild != null || MiddleChild != null || HighChild != null;

            public bool IsLowChild => Parent != null && ReferenceEquals(Parent.LowChild, this);

            public bool IsHighChild => Parent != null && ReferenceEquals(Parent.HighChild, this);

            private bool IsMiddleChild => Parent != null && ReferenceEquals(Parent.MiddleChild, this);

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

        #region Public Interface

        public int Count { get; private set; }

        public IEnumerable<string> Keys
        {
            get
            {
                var keys = new List<string>(Count);
                GetKeys(_root, keys);
                return keys;
            }
        }

        public bool Any(string prefix)
        {
            var node = Find(prefix);
            return node != null;
        }

        public bool Single(string key)
        {
            var node = Find(key);
            return node != null && node.IsKey;
        }

        /// <summary>
        ///     Adds the specified prefix to the tree.
        /// </summary>
        /// <param name="key">The prefix to add.</param>
        /// <returns>True, if the prefix could be added; otherwise false</returns>
        /// <exception cref="ArgumentNullException">"Invalid prefix specified (null or empty).</exception>
        public bool TryAdd(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            // create root node if necessary.		
            if (_root == null)
                _root = new Node(null, key[0]);

            // add prefix
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
                            //throw new ArgumentException("prefix already in dictionary.");
                            return false;
                        break;
                    }

                    if (node.MiddleChild == null)
                        node.MiddleChild = new Node(node, key[i]);
                    node = node.MiddleChild;
                }
            }

            node.IsKey = true;
            node.Key = key;
            Count++;

            return true;
        }

        /// <summary>
        ///     Adds a range of keys using a recursive partioning around the median element strategy
        ///     which is an fair approximation to keeping the tree balanced.
        ///     See the paper "Fast Algorithms for Sorting and Searching Keys" of Bentley and Sedgewick,
        ///     e.g. at http://www.cs.princeton.edu/~rs/strings/
        /// </summary>
        /// <param name="keys">The keys to add.</param>
        /// <param name="needSorting">if set to <c>true</c> input values will be sorted in ascending prefix order.</param>
        public void AddRange(IEnumerable<string> keys, bool needSorting = true)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));
            var kys = keys.ToArray();
            if (kys.Length <= 0) return;
            if (needSorting)
                Array.Sort(kys);
            AddBalanced(kys, 0, kys.Length);
        }

        /// <summary>
        ///     Removes the specified prefix from the tree.
        /// </summary>
        /// <param name="key">The prefix to remove.</param>
        /// <returns>True, if the prefix could be removed; otherwise false</returns>
        /// <exception cref="ArgumentNullException">Invalid prefix string specified (null or empty).</exception>
        public bool TryRemove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Invalid prefix string specified (null or empty).");

            var node = Find(key);
            if (node == null)
                return false;

            if (node.IsKey)
                Count--;

            // destroy prefix (!)
            node.IsKey = false;
            node.Key = null;

            // remove all single linked parent nodes up the hierarchy
            while (!node.IsKey && !node.HasChildren && node.Parent != null)
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

        /// <summary>
        ///     Removes all elements from the tree.
        /// </summary>
        public void Clear()
        {
            Count = 0;
            _root = null;
        }

        /// <summary>
        ///     Gets all keys matching the specified prefix. If a null prefix is specified, all keys are returned.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns />
        public IEnumerable<string> KeysMatching(string prefix)
        {
            var matches = new List<string>();
            var node = Find(prefix);
            if (node != null)
            {
                if (node.IsKey)
                    matches.Add(node.Key);
                GetKeys(node.MiddleChild, matches);
            }

            return matches;
        }

        /// <summary>
        ///     Partial match search with wild-char character.
        /// </summary>
        /// <param name="pattern">text pattern to match</param>
        /// <param name="wildChar">"don't care" character</param>
        /// <returns />
        /// <exception cref="ArgumentNullException">Invalid prefix string specified (null or empty).</exception>
        public IEnumerable<string> KeysLike(string pattern, char wildChar = '*')
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(nameof(pattern));

            var matches = new List<string>();
            PartialMatch(_root, pattern, 0, wildChar, matches);
            return matches;
        }

        /// <summary>
        ///     Near-neighbor search in the tree.
        /// </summary>
        /// <param name="pattern">The pattern to search neighbors for.</param>
        /// <param name="distance">The maximum allowed hamming distance to the pattern.</param>
        /// <returns>The neighbors within <paramref name="distance" /> to the pattern.</returns>
        /// <exception cref="ArgumentNullException">Invalid pattern (null or empty).</exception>
        /// <exception cref="ArgumentOutOfRangeException">Negative distance specified.</exception>
        public IEnumerable<string> KeysNearBy(string pattern, int distance)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(nameof(pattern));
            if (distance < 0)
                throw new ArgumentOutOfRangeException(nameof(distance));

            var matches = new List<string>();
            NearNeighbors(_root, pattern, 0, distance, matches);
            return matches;
        }

        #endregion

        #region Methods

        private void AddBalanced(IList<string> keys, int left, int right)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (left >= right)
                return;
            var pivot = (left + right) / 2;
            TryAdd(keys[pivot]);
            AddBalanced(keys, left, pivot);
            AddBalanced(keys, pivot + 1, right);
        }

        /// <summary>
        ///     Finds the closest node for the specified prefix.
        /// </summary>
        /// <returns>The node closest to the specified prefix, null if not found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="prefix" /> is null or empty.</exception>
        private Node Find(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
            var node = _root;
            var index = 0;
            while (index < prefix.Length && node != null)
            {
                var c = prefix[index];
                if (c < node.SplitChar)
                {
                    node = node.LowChild;
                }
                else if (c > node.SplitChar)
                {
                    node = node.HighChild;
                }
                else // c == node.SplitChar
                {
                    if (index + 1 == prefix.Length) // found!
                        return node;
                    ++index;
                    node = node.MiddleChild;
                }
            }

            return null;
        }

        private static void GetKeys(Node node, ICollection<string> keys)
        {
            if (node == null) return;
            GetKeys(node.LowChild, keys);
            if (node.IsKey)
                keys.Add(node.Key);
            else
                GetKeys(node.MiddleChild, keys);
            GetKeys(node.HighChild, keys);
        }

        private static void PartialMatch(Node node, string pattern, int index,
            char wildChar, ICollection<string> matches)
        {
            if (node == null)
                return;

            var c = pattern[index];
            if (c == wildChar || c < node.SplitChar)
                PartialMatch(node.LowChild, pattern, index, wildChar, matches);

            if (c == wildChar || c == node.SplitChar)
            {
                if (index < pattern.Length - 1)
                    PartialMatch(node.MiddleChild, pattern, index + 1, wildChar, matches);
                else if (node.IsKey)
                    matches.Add(node.Key);
            }

            if (c == wildChar || c > node.SplitChar)
                PartialMatch(node.HighChild, pattern, index, wildChar, matches);
        }

        private static void NearNeighbors(Node node, string pattern, int index,
            int dist, ICollection<string> matches)
        {
            if (node == null || dist < 0)
                return;

            var c = pattern[index];

            // low child
            if (dist > 0 || c < node.SplitChar)
                NearNeighbors(node.LowChild, pattern, index, dist, matches);

            // middle child
            if (node.IsKey)
            {
                if (pattern.Length <= dist + index)
                    matches.Add(node.Key);
            }
            else
            {
                var localIndex = index;
                if (localIndex != pattern.Length - 1)
                    ++localIndex;
                var localDist = dist;
                if (c != node.SplitChar)
                    --localDist;

                NearNeighbors(node.MiddleChild, pattern, localIndex, localDist, matches);
            }

            // high child
            if (dist > 0 || c > node.SplitChar)
                NearNeighbors(node.HighChild, pattern, index, dist, matches);
        }

        #endregion
    }
}