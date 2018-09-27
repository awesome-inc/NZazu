using System.Collections.Generic;

namespace NZazu.Contracts.Suggest
{
    public interface IHaveKeyStrings
    {
        /*
        ///<summary>
        /// Gets the number of key strings contained in this instance.
        /// </summary>	
        /// <value />
        int Count { get; }

        /// <summary>
        /// Gets all strings.
        /// </summary>
        /// <value />
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Removes all strings.
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Determines whether this instance contains the specified key.
        /// </summary>
        /// <param name="key">The key string.</param>
        /// <returns />
        bool Single(string key);
         */

        /// <summary>
        /// Gets all keys starting with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns />
        IEnumerable<string> KeysMatching(string prefix);

        /*
        /// <summary>
        /// Gets all suffixes around the specified pattern within the specified hamming distance.
        /// </summary>
        /// <param name="pattern">The pattern whose neighbours to search for.</param>
        /// <param name="distance">The maximum hamming distance to the specified pattern.</param>
        /// <returns>The Neighbors within <paramref name="distance"/> to the specified pattern.</returns>
        IEnumerable<string> KeysNearBy(string pattern, int distance);

        /// <summary>
        /// Gets all suffixes matching the specified pattern which may contain wildcard characters.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="wildChar">The wildcard character.</param>
        /// <returns />
        IEnumerable<string> KeysLike(string pattern, char wildChar = '*');
         */
    }
}