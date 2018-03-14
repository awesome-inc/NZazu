using System;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(PropertyExtensions))]
    // ReSharper disable InconsistentNaming
    internal class PropertyExtensions_Should
    {
        [Test]
        public void Support_child_properties()
        {
            var parent = new DummyParent {Child = new DummyChild {Name = "name"}};

            var propName = $"{nameof(DummyParent.Child)}.{nameof(DummyChild.Name)}";
            parent.CanSetProperty(propName).Should().BeTrue();

            const string expected = "John";
            parent.SetProperty(propName, expected);
            parent.Child.Name.Should().Be(expected);

            parent.Invoking(x => x.CanSetProperty("foo.bar"))
                .Should().Throw<ArgumentException>()
                .WithMessage("Property 'foo.bar' does not exist.");

            parent.Invoking(x => x.SetProperty("foo.bar", "value"))
                .Should().Throw<ArgumentException>()
                .WithMessage("Property 'foo.bar' does not exist.");

            parent.Invoking(x => x.SetProperty("foo", "value"))
                .Should().Throw<ArgumentException>()
                .WithMessage("Property 'foo' does not exist.");
        }

        private class DummyParent
        {
            public DummyChild Child { get; set; }
        }

        private class DummyChild
        {
            public string Name { get; set; }
        }
    }
}