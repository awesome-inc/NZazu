using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Extensions
{
    [TestFixtureFor(typeof(FocusExtensions))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class FocusExtensions_Should
    {
        [Test]
        [STAThread]
        public void SetFocus()
        {
            var control = new TextBox();
            control.IsFocused.Should().BeFalse();

            control.SetFocus();

            control.IsFocused.Should().BeTrue();
        }
    }
}