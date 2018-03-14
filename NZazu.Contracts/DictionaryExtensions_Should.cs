using System.Collections.Generic;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [TestFixtureFor(typeof (DictionaryExtensions))]
    // ReSharper disable once InconsistentNaming
    internal class DictionaryExtensions_Should
    {
        [Test]
        public void Remove_Items()
        {
            var source = new Dictionary<string, string> { { "name", "thomas" }, { "street", "123 Ave" }, { "nullthing", null } };

            var expected1 = new Dictionary<string, string> { { "name", "thomas" }, { "street", "123 Ave" } };
            var actual1 = source.Remove(kvp => kvp.Value == null);
            actual1.Should().BeEquivalentTo(expected1);

            var expected2 = new Dictionary<string, string> { { "street", "123 Ave" }, { "nullthing", null } };
            var actual2 = source.Remove(kvp => kvp.Key == "name");
            actual2.Should().BeEquivalentTo(expected2);

            var actual3 = source.Remove(kvp => true);
            actual3.Should().BeEmpty();

            var actual4 = source.Remove(kvp => false);
            actual4.Should().BeEquivalentTo(source);
        }

        [Test]
        public void Test_Count_and_pairs_on_Equivalent()
        {
            var dict = new Dictionary<string, string>();
            var dict2 = new Dictionary<string, string> { {"1","1"}};

            dict.Equivalent(dict2).Should().BeFalse("count differs");

            dict["2"] = "2";
            dict.Equivalent(dict2).Should().BeFalse("key not present");

            dict["1"] = "1";
            dict2["2"] = "2";
            dict.Equivalent(dict2).Should().BeTrue();
        }

        [Test]
        public void MergeWith_should_add_or_update_but_not_remove_values()
        {
            var dst = new Dictionary<string, string>();
            var src = new Dictionary<string, string> { { "key", "value" } };
            dst.MergeWith(src).Should().BeTrue();
            dst["key"].Should().Be(src["key"], "field added");

            src["key"] = "value2";
            dst.MergeWith(src).Should().BeTrue(" field updated");
            dst["key"].Should().Be(src["key"]);

            dst.MergeWith(src).Should().BeFalse("no changes");
            dst["key"].Should().Be(src["key"]);

            dst["key2"] = "value2";
            dst.MergeWith(src).Should().BeFalse("no changes");
            dst["key2"].Should().Be("value2");

            src["key"] = "value";
            dst.MergeWith(src).Should().BeTrue(" field updated");
            dst["key"].Should().Be(src["key"]);
            dst["key2"].Should().Be("value2");
        }

        [Test]
        public void MergedWith_should_add_or_update_fields()
        {
            var src = new Dictionary<string, string> { { "key", "value" } };
            var dst = ((Dictionary<string, string>)null).MergedWith(src);
            dst.Should().BeEquivalentTo(src, "null is not only allowed but also expected");

            dst["key"] = "value2";
            dst = dst.MergedWith(src);
            dst.Should().BeEquivalentTo(src, "src field should be updated");

            dst["key2"] = "value2";
            dst = dst.MergedWith(src);
            dst["key"].Should().Be(src["key"]);
            dst["key2"].Should().Be("value2");
        }
    }
}