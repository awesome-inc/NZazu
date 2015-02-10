using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [TestFixture]
// ReSharper disable once InconsistentNaming
    class DictionaryExtensions_Should
    {
        [Test]
        public void Remove_Items()
        {
            var source = new Dictionary<string, string> { { "name", "thomas" }, { "street", "123 Ave" }, { "nullthing", "null" } };

            var expected1 = new Dictionary<string, string> { { "name", "thomas" }, { "street", "123 Ave" } };
            var actual1 = source.Remove(kvp => kvp.Value == null);
            actual1.ShouldAllBeEquivalentTo(expected1);

            var expected2 = new Dictionary<string, string> { { "street", "123 Ave" }, { "nullthing", "null" } };
            var actual2 = source.Remove(kvp => kvp.Key == "name");
            actual2.ShouldAllBeEquivalentTo(expected2);

            var actual3 = source.Remove(kvp => true);
            actual3.Should().BeEmpty();

            var actual4 = source.Remove(kvp => false);
            actual4.ShouldAllBeEquivalentTo(source);
        }
    }
}