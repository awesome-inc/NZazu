using System.Linq;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;

namespace NZazu.Contracts.Suggest
{
    [TestFixtureFor(typeof(AggregateProvideSuggestions))]
    // ReSharper disable once InconsistentNaming
    internal class AggregateProvideSuggestions_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new AggregateProvideSuggestions(Enumerable.Empty<IProvideSuggestions>());

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<IProvideSuggestions>();
        }

        [Test]
        public void Merge_Provider()
        {
            var p1 = Substitute.For<IProvideSuggestions>();
            p1.For(Arg.Any<string>(), Arg.Any<string>())
                .ReturnsForAnyArgs(new[] {"A", "B"});
            var p2 = Substitute.For<IProvideSuggestions>();
            p2.For(Arg.Any<string>(), Arg.Any<string>())
                .ReturnsForAnyArgs(new[] {"1", "2"});

            var sut = new AggregateProvideSuggestions(new[] {p1, p2});

            var sug = sut.For("jo", "cs").ToArray();

            p1.Received(1).For(Arg.Any<string>(), Arg.Any<string>());
            p2.Received(1).For(Arg.Any<string>(), Arg.Any<string>());

            sug.Should().Contain("A");
            sug.Should().Contain("B");
            sug.Should().Contain("1");
            sug.Should().Contain("2");
        }

        [Test]
        public void Handle_Null_Result()
        {
            var p1 = Substitute.For<IProvideSuggestions>();
            p1.For(Arg.Any<string>(), Arg.Any<string>())
                .ReturnsForAnyArgs(new[] {"A", "B"});
            var p2 = Substitute.For<IProvideSuggestions>();
            p2.For(Arg.Any<string>(), Arg.Any<string>())
                .ReturnsForAnyArgs((string[]) null);

            var sut = new AggregateProvideSuggestions(new[] {p1, p2});

            var sug = sut.For("jo", "cs").ToArray();

            p1.Received(1).For(Arg.Any<string>(), Arg.Any<string>());
            p2.Received(1).For(Arg.Any<string>(), Arg.Any<string>());

            sug.Should().Contain("A");
            sug.Should().Contain("B");
        }
    }
}