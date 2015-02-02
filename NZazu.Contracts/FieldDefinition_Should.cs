using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    class FieldDefinition_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new FieldDefinition();

            sut.Should().NotBeNull();
        }
    }
}