using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Suggest
{
    [TestFixtureFor(typeof(ProvideFileSuggestions))]
    // ReSharper disable once InconsistentNaming
    internal class ProvideFileSuggestions_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new ProvideFileSuggestions();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<IProvideSuggestions>();
        }

        [Test]
        public void Return_Prefixes()
        {
            const string dataConnection = "file://cities.txt";
            var prefix = "an";
            var expected = new string[] { };
            var sut = new ProvideFileSuggestions();

            var actual = sut.For(prefix, dataConnection);
            actual.Should().BeEquivalentTo(expected);

            prefix = "";
            expected = new string[] { };
            actual = sut.For(prefix, dataConnection);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Return_Empty_If_Not_Match()
        {
            const string dataConnection = "this is a crappy data connection";
            var prefix = "thomas";
            var expected = new string[] { };
            var sut = new ProvideFileSuggestions();

            var actual = sut.For(prefix, dataConnection);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}