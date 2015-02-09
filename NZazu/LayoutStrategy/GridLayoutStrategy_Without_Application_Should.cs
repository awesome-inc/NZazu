using FluentAssertions;
using NUnit.Framework;

namespace NZazu.LayoutStrategy
{
    [TestFixture, RequiresSTA]
    // ReSharper disable InconsistentNaming
    class GridLayoutStrategy_Without_Application_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new GridLayoutStrategy();
            sut.Should().NotBeNull();
        }
    }
}