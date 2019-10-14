using System.Linq;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Suggest
{
    [TestFixtureFor(typeof(Cache<int, int>))]
    // ReSharper disable InconsistentNaming
    internal class Cache_Should
    {
        [Test]
        public void Skip_lru_items()
        {
            const int maxSize = 5;
            var sut = new Cache<int, int>(maxSize);
            Enumerable.Range(0, maxSize).ToList().ForEach(i =>
            {
                sut.Add(i, i);
                sut.ContainsKey(i).Should().BeTrue();
            });

            sut.Add(maxSize, maxSize);
            sut.ContainsKey(0).Should().BeFalse("lru item should be bumped from cache");
        }
    }
}