using FluentAssertions;
using NUnit.Framework;
using NZazu.LayoutStrategy;

namespace NZazu
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    class ResolveLayout_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new ResolveLayout();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<IResolveLayout>();
            sut.Should().BeAssignableTo<IResolveFromName<INZazuWpfLayoutStrategy>>();
        }

        [Test]
        public void Have_GridLayout_As_Default()
        {
            var sut = new ResolveLayout();

            sut.Resolve(null).Should().BeAssignableTo<GridLayout>();
        }

        [Test]
        public void Resolve_StackedLayout()
        {
            var sut = new ResolveLayout();

            sut.Resolve("stack").Should().BeAssignableTo<StackedLayout>();
        }
    }
}