using System.Linq;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(AggregateValueCheckResult))]
    internal class AggregateValueCheckResult_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new AggregateValueCheckResult(Enumerable.Empty<ValueCheckResult>());

            sut.Should().NotBeNull();
        }
    }
}