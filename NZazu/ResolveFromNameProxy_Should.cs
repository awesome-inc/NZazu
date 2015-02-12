using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace NZazu
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    class ResolveFromNameProxy_Should
    {
        [Test]
        public void Be_Creatable()
        {
            Func<string, object> func = s => new object();
            var sut = new ResolveFromNameProxy<object>(func);

            sut.Should().NotBeNull();
            sut.Resolve("foo").Should().NotBeNull();
        }
        [Test]
        public void Resolve_Using_Function()
        {
            var func = Substitute.For<Func<string, object>>();
            var sut = new ResolveFromNameProxy<object>(func);

            sut.Resolve("foo");
            func.Received().Invoke("foo");
        }
    }
}