using System.Collections.Generic;
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

        [Test]
        public void Be_Symetric()
        {
            var data = new Dictionary<string, string>
            {
                {"Jane", "Doe"},
                {"John", "Smith"},
            };

            var sut = new NZazuXmlSerializer();

            var actual = sut.Serialize(data);
            var expected = sut.Deserialize(actual);

            foreach (var item in data)
                expected.Should().Contain(item);
        }
    }
}