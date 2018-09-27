using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Suggest
{
    [TestFixtureFor(typeof(SuggestionsProxy))]
    // ReSharper disable once InconsistentNaming
    internal class SuggestionsProxy_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var ctx = new ContextFor<SuggestionsProxy>();
            var sut = ctx.BuildSut();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<IProvideSuggestions>();
        }
    }
}