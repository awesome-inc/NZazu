using System;
using System.Windows.Controls;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace NZazu.FieldBehavior
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable once InconsistentNaming
    class EmptyNZazuFieldBehavior_Should
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
        public void Throw_Excepion_If_No_Control_Provided()
        {
            var sut = new EmptyNZazuFieldBehavior();

            new Action(() => sut.AttachTo(null))
                .Invoking(a => a())
                .ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Do_Nothing()
        {
            var ctrl = Substitute.For<Control>();
            var sut = new EmptyNZazuFieldBehavior();

            sut.AttachTo(ctrl);
            sut.Detach();

            ctrl.ReceivedCalls().Should().BeEmpty();
        }
    }
}