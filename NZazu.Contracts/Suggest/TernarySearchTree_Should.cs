using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Suggest
{
    [TestFixtureFor(typeof(TernarySearchTree))]
    // ReSharper disable once InconsistentNaming
    internal class TernarySearchTree_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var ctx = new ContextFor<TernarySearchTree>();
            var sut = ctx.BuildSut();

            sut.Should().NotBeNull();
        }
    }
}