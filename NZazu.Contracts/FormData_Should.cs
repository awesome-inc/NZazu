using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class FormData_Should
    {
        [Test]
        public void Support_implicit_casting_from_and_to_dictionaries()
        {
            var input = new Dictionary<string, string> {{"user","John"}};
            FormData formData = input;
            formData.Values.ShouldBeEquivalentTo(input);

            Dictionary<string,string> output = formData;

            output.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void Implement_Equals_based_on_comparing_the_values_map()
        {
            FormData formData1 = new Dictionary<string, string> { { "user", "John" } };
            FormData formData2 = new Dictionary<string, string> { { "user", "John" } };

            formData1.Equals(formData2).Should().BeTrue();

            formData2.Values["user"] = "Jim";

            formData1.Should().NotBe(formData2);
        }
    }
}