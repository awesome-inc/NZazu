using System;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts.Checks;

namespace NZazu.Extensions
{
    [TestFixtureFor(typeof(ViewExtensions))]
    // ReSharper disable InconsistentNaming
    internal class ViewExtensions_Should
    {
        [Test]
        public void Return_False_If_Validate_Has_Exception()
        {
            var view = Substitute.For<INZazuWpfView>();
            view.Validate().Returns(new ValueCheckResult(false, new Exception("I am invalid")));

            view.IsValid().Should().BeFalse();

            view.ReceivedWithAnyArgs().Validate();
        }

        [Test]
        public void Return_True_If_Validate()
        {
            var view = Substitute.For<INZazuWpfView>();
            view.Validate().Returns(ValueCheckResult.Success);

            view.IsValid().Should().BeTrue();

            view.ReceivedWithAnyArgs().Validate();
        }
    }
}