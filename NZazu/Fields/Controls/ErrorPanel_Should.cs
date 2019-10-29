using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Fields.Controls
{
    [TestFixtureFor(typeof(ErrorPanel))]
    // ReSharper disable once InconsistentNaming
    internal class ErrorPanel_Should
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Be_A_Control()
        {
            var sut = new ErrorPanel();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<UserControl>();
        }
    }
}