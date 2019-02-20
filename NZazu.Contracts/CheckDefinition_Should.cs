using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using System.Collections.Generic;

namespace NZazu.Contracts
{
    [TestFixtureFor(typeof(CheckDefinition))]
    // ReSharper disable InconsistentNaming
    internal class CheckDefinition_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var settings = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" } };
            var sut = new CheckDefinition { Type = "type", Settings = settings };
            sut.Type.Should().Be("type");
            sut.Settings.Should().BeEquivalentTo(settings);
        }
    }
}