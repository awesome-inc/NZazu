using System;

namespace NZazu.Contracts.Suggest
{
    /// <summary>
    ///     An generic blacklist interface.
    /// </summary>
    public interface IBlackList<in TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Gets or sets the maximum size of the black list (number of keys).
        /// </summary>
        /// <value>
        ///     The size of the black list.
        /// </value>
        int Capacity { get; set; }

        /// <summary>
        ///     Gets the current number of items in the blacklist.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Clears the blacklist.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Pops a retreival failure for the specified key from the blacklist.
        /// </summary>
        /// <param name="key">The key to pop.</param>
        /// <returns></returns>
        bool Pop(TKey key);

        /// <summary>
        ///     Pushes a retreival failure for the specified key to the blacklist.
        /// </summary>
        /// <param name="key">The key to push.</param>
        /// <returns></returns>
        bool Push(TKey key);

        /// <summary>
        ///     Determines whether the specified key is blacklisted by comparing the number
        ///     of pushes against <paramref name="maxFails" />.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="maxFails">The maximum numbers of failure to allow before blacklisted.</param>
        /// <returns>
        ///     <c>true</c> if the specified key is black-listed; otherwise, <c>false</c>.
        /// </returns>
        bool IsBlackListed(TKey key, int maxFails = 1);
    }
}