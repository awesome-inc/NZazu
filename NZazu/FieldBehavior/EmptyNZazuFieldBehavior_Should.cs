using System;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;

namespace NZazu.FieldBehavior
{
    [TestFixtureFor(typeof (EmptyNZazuFieldBehavior))]
    // ReSharper disable once InconsistentNaming
    internal class EmptyNZazuFieldBehavior_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new EmptyNZazuFieldBehavior();
            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfFieldBehavior>();
            sut.GetType().Name.Should().StartWith("Empty");
        }

        [Test]
        public void Have_guard_clauses_on_ctor()
        {
            var sut = new EmptyNZazuFieldBehavior();

            var field = Substitute.For<INZazuWpfField>();
            var view = Substitute.For<INZazuWpfView>();
            sut.Invoking(x => x.AttachTo(null,null)).ShouldThrow<ArgumentNullException>();
            sut.Invoking(x => x.AttachTo(field, null)).ShouldThrow<ArgumentNullException>();
            sut.Invoking(x => x.AttachTo(null, view)).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Do_Nothing()
        {
            var field = Substitute.For<INZazuWpfField>();
            var view = Substitute.For<INZazuWpfView>();
            var sut = new EmptyNZazuFieldBehavior();

            sut.AttachTo(field, view);
            sut.Detach();

            field.ReceivedCalls().Should().BeEmpty();
            view.ReceivedCalls().Should().BeEmpty();
        }
    }
}