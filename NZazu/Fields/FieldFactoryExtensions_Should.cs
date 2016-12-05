using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(FieldFactoryExtensions))]
    // ReSharper disable once InconsistentNaming
    internal class FieldFactoryExtensions_Should
    {
        [Test]
        public void Be_Static()
        {
            var type = typeof(FieldFactoryExtensions);

            type.IsAbstract.Should().BeTrue();
            type.IsSealed.Should().BeTrue();
        }
    }
}