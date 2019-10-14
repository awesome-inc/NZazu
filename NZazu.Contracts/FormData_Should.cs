using System.Collections.Generic;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [TestFixtureFor(typeof(FormData))]
    // ReSharper disable InconsistentNaming
    internal class FormData_Should
    {
        [Test]
        public void Support_implicit_casting_from_and_to_dictionaries()
        {
            var input = new Dictionary<string, string> {{"user", "John"}};
            FormData formData = input;
            formData.Values.Should().BeEquivalentTo(input);

            Dictionary<string, string> output = formData;

            output.Should().BeEquivalentTo(input);
        }

        [Test]
        public void Not_Serialize_Null_Values()
        {
            var expected = new Dictionary<string, string> {{"user", "john"}};
            var input = new Dictionary<string, string> {{"user", "john"}, {"password", null}};

            FormData formData = input;
            formData.Values.Should().BeEquivalentTo(expected);

            Dictionary<string, string> output = formData;
            output.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Implement_Equals_based_on_comparing_the_values_map()
        {
            FormData formData1 = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}};
            FormData formData2 = new Dictionary<string, string> {{"key2", "value2"}, {"key1", "value1"}};

            formData1.Equals(formData2).Should().BeTrue();
            formData1.Equals((object) formData2).Should().BeTrue();

            formData2.Values["key1"] = "value2";

            formData1.Equals(formData2).Should().BeFalse();
        }
    }
}