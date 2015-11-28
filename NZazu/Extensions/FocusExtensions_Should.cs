using System;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Extensions
{
    [TestFixtureFor(typeof (FocusExtensions))]
    // ReSharper disable InconsistentNaming
    internal class FocusExtensions_Should
    {
        [Test]
        [STAThread]
        public void SetFocus()
        {
            var control = new TextBox();
            control.IsFocused.Should().BeFalse();

            control.Focus();

            control.IsFocused.Should().BeTrue();
        }
    }
}