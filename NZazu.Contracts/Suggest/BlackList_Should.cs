using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NSuggest
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
    }
}