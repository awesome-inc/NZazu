using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [TestFixtureFor(typeof (CheckDefinition))]
    // ReSharper disable InconsistentNaming
    internal class CheckDefinition_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var values = new[]{"value1", "value2"};
            var sut = new CheckDefinition {Type = "type", Values = values};
            sut.Type.Should().Be("type");
            sut.Values.Should().BeEquivalentTo(values);
        }
    }
}