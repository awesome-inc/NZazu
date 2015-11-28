using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.LayoutStrategy
{
    [ExcludeFromConventions("special test - avoid test setup")]
    [TestFixture]
    // ReSharper disable InconsistentNaming
    internal class GridLayout_Without_Application_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new GridLayout();
            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfLayoutStrategy>();
        }
    }
}