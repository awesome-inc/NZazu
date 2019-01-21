using System;
using System.Collections.Generic;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts.Checks;

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
            sut.Invoking(x => x.CreateCheck(null)).Should().Throw<ArgumentNullException>();
            sut.Invoking(x => x.CreateCheck(new CheckDefinition())).Should().Throw<ArgumentException>().WithMessage("check type not specified");
            sut.Invoking(x => x.CreateCheck(new CheckDefinition { Type = "foobar" })).Should().Throw<NotSupportedException>().WithMessage("The specified check 'foobar' is not supported");
        }
    }
}