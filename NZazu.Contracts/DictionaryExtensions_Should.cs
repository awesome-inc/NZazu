using System.Collections.Generic;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [TestFixtureFor(typeof(DictionaryExtensions))]
    // ReSharper disable once InconsistentNaming
    internal class DictionaryExtensions_Should
    {
        [Test]
        public void Add_Or_Replace()
        {
            var source = new Dictionary<string, string>
                {{"name", "thomas"}, {"street", "123 Ave"}, {"nullthing", null}};

            source.AddOrReplace("name", "horst").Should().Contain("name", "horst");
            source.AddOrReplace("street", "456 Ave").Should().Contain("street", "456 Ave");

            // handle null to return new dict with the parameter
            ((Dictionary<string, string>) null).AddOrReplace("name", "horst").Should().Contain("name", "horst");
        }

        [Test]
        public void Remove_Items()
        {
            var source = new Dictionary<string, string>
                {{"name", "thomas"}, {"street", "123 Ave"}, {"nullthing", null}};

            var expected1 = new Dictionary<string, string> {{"name", "thomas"}, {"street", "123 Ave"}};
            var actual1 = source.Remove(kvp => kvp.Value == null);
            actual1.Should().BeEquivalentTo(expected1);

            var expected2 = new Dictionary<string, string> {{"street", "123 Ave"}, {"nullthing", null}};
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
            var dict2 = new Dictionary<string, string> {{"1", "1"}};

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
            var src = new Dictionary<string, string> {{"key", "value"}};
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
            var src = new Dictionary<string, string> {{"key", "value"}};
            var dst = ((Dictionary<string, string>) null).MergedWith(src);
            dst.Should().BeEquivalentTo(src, "null is not only allowed but also expected");

            dst["key"] = "value2";
            dst = dst.MergedWith(src);
            dst.Should().BeEquivalentTo(src, "src field should be updated");

            dst["key2"] = "value2";
            dst = dst.MergedWith(src);
            dst["key"].Should().Be(src["key"]);
            dst["key2"].Should().Be("value2");
        }

        [Test]
        public void Get_From_Dict()
        {
            var src = new Dictionary<string, string> {{"key", "value"}};
            src.Get("key").Should().Be("value");
            src.Get("foo").Should().Be(null);
            src.Get("foo", "value2").Should().Be("value2");
        }

        [Test]
        public void Get_Generic_From_Dict()
        {
            var src = new Dictionary<string, string>
            {
                {"key", "100"},
                {"key2", "200.4"}
            };
            src.Get<double>("key").Should().Be(100);
            src.Get<double>("key2").Should().Be(200.4);
            src.Get<double>("foo").Should().Be(null);
        }

        [Test]
        public void Object_To_Dictionary()
        {
            var obj = new
            {
                Name = "Thomas",
                Age = 42
            };

            var dict = obj.AsDictionary();
            dict["Name"].Should().Be("Thomas");
            dict["Age"].Should().Be(42);
        }

        [Test]
        public void TestDummy_Dictionary_To_Object()
        {
            var dict = new Dictionary<string, object>
            {
                {"Name", "Thomas"},
                {"Age", 42}
            };

            var obj = dict.ToObject<TestDummy>();
            obj.Name.Should().Be("Thomas");
            obj.Age.Should().Be(42);
        }

        [Test]
        public void TestDummy_Object_To_Dictionary()
        {
            var obj = new TestDummy
            {
                Name = "Thomas",
                Age = 42
            };

            var dict = obj.AsDictionary();
            dict["Name"].Should().Be("Thomas");
            dict["Age"].Should().Be(42);
        }

        [Test]
        public void Anonymous_Object_To_Dictionary()
        {
            var obj = new
            {
                Name = "Thomas",
                Age = 42
            };

            var dict = obj.AsDictionary();
            dict["Name"].Should().Be("Thomas");
            dict["Age"].Should().Be(42);
        }

        private class TestDummy
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}