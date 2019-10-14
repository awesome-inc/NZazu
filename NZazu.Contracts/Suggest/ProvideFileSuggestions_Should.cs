using System.Linq;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
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
            var mock = Substitute.For<IFileSystem>();
            mock.ReadAllLines("cities.txt").Returns("Bonn|Cologne|Bern|Berlin".Split('|'));
            mock.FileExists("cities.txt").Returns(true);

            var prefix = "Co";
            var expected = new[] {"Cologne"};
            var sut = new ProvideFileSuggestions(mock);

            var actual = sut.For(prefix, dataConnection);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Return_Empty_On_Empty_Prefix()
        {
            const string dataConnection = "file://cities.txt";
            var mock = Substitute.For<IFileSystem>();
            mock.ReadAllLines("cities.txt").Returns("Bonn|Cologne|Bern|Berlin".Split('|'));
            mock.FileExists("cities.txt").Returns(true);

            var prefix = "";
            var expected = new string[] { };
            var sut = new ProvideFileSuggestions(mock);

            var actual = sut.For(prefix, dataConnection);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Return_Prefixes_Twice_Uses_Caching()
        {
            const string dataConnection = "file://cities.txt";
            const string prefix = "Be";
            var expected = new[] {"Bern", "Berlin"};
            var mock = Substitute.For<IFileSystem>();
            mock.ReadAllLines("cities.txt").Returns("Bonn|Cologne|Bern|Berlin".Split('|'));
            mock.FileExists("cities.txt").Returns(true);

            var sut = new ProvideFileSuggestions(mock);

            var temp = sut.For(prefix, dataConnection).ToArray();
            var actual = sut.For(prefix, dataConnection).ToArray();

            actual.Should().BeEquivalentTo(expected);
            actual.Should().BeEquivalentTo(temp, "the results should be equal if you all it twice");

            mock.Received(1).ReadAllLines("cities.txt");
            mock.Received(1).FileExists("cities.txt");
        }

        [Test]
        public void Return_Empty_If_Not_Exists()
        {
            const string dataConnection = "file://cities.txt";
            var mock = Substitute.For<IFileSystem>();
            mock.ReadAllLines("cities.txt").Returns(Enumerable.Empty<string>());
            mock.FileExists("cities.txt").Returns(false);

            var prefix = "thomas";
            var expected = new string[] { };
            var sut = new ProvideFileSuggestions(mock);

            var actual = sut.For(prefix, dataConnection);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}