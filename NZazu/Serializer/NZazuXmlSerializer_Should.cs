using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Serializer
{
    [TestFixtureFor(typeof(NZazuXmlSerializer))]
    // ReSharper disable once InconsistentNaming
    internal class NZazuXmlSerializer_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuXmlSerializer();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuDataSerializer>();
        }
    }
}