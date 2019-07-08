using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Suggest
{
    [TestFixtureFor(typeof(BlackList<>))]
    // ReSharper disable once InconsistentNaming
    internal class BlackList_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var ctx = new ContextFor<BlackList<string>>();
            ctx.Use(10);
            var sut = ctx.BuildSut();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<IBlackList<string>>();
        }

        [Test]
        public void Push_And_Pop()
        {
            var ctx = new ContextFor<BlackList<string>>();
            ctx.Use(10);
            var sut = ctx.BuildSut();

            sut.Push("foo");
            sut.Push("bar");
            sut.Count.Should().Be(2);

            sut.Push("bar");
            sut.Count.Should().Be(2);

            sut.Pop("foo").Should().BeTrue();
            sut.Count.Should().Be(1);

            sut.Pop("bar").Should().BeTrue();
            sut.Count.Should().Be(0);

            sut.Pop("bar").Should().BeFalse();
            sut.Pop("hugo").Should().BeFalse();
        }

        [Test]
        public void Clear()
        {
            var ctx = new ContextFor<BlackList<string>>();
            ctx.Use(10);
            var sut = ctx.BuildSut();

            sut.Push("foo");
            sut.Push("bar");

            sut.Clear();
            sut.Count.Should().Be(0);
            sut.ToString().Should().Contain("Cur/Max = 0/10");

            sut.Pop("foo").Should().BeFalse();
            sut.Pop("bar").Should().BeFalse();
            sut.Pop("hugo").Should().BeFalse();
        }
    }
}