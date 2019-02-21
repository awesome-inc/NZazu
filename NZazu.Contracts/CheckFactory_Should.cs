using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using System;

namespace NZazu.Contracts
{
    [TestFixtureFor(typeof(CheckFactory))]
    // ReSharper disable InconsistentNaming
    internal class CheckFactory_Should
    {
        [Test]
        public void Throw_on_unsupported_types()
        {
            var sut = new CheckFactory();
            sut.Invoking(x => x.CreateCheck(null, null)).Should().Throw<ArgumentNullException>();
            sut.Invoking(x => x.CreateCheck(new CheckDefinition(), null)).Should().Throw<ArgumentException>().WithMessage("check type not specified");
            sut.Invoking(x => x.CreateCheck(new CheckDefinition { Type = "foobar" }, null)).Should().Throw<NotSupportedException>().WithMessage("The specified check 'foobar' is not supported");
        }
    }
}