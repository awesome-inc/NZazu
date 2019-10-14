using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException

namespace NZazu.FieldBehavior
{
    [TestFixtureFor(typeof(SetBorderBehavior))]
    // ReSharper disable once InconsistentNaming
    internal class SetBorderBehavior_Should
    {
        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Set_Border()
        {
            var sut = new SetBorderBehavior();
            var field = Substitute.For<INZazuWpfField>();
            field.ValueControl.Returns(Substitute.For<Control>());
            sut.AttachTo(field, Substitute.For<INZazuWpfView>());

            var control = sut.Control;
            control.BorderBrush.Should().BeOfType<SolidColorBrush>();
            (control.BorderBrush as SolidColorBrush).Color.A.Should().Be(SystemColors.HotTrackColor.A);
            (control.BorderBrush as SolidColorBrush).Color.A.Should().Be(SystemColors.HotTrackColor.A);
            (control.BorderBrush as SolidColorBrush).Color.A.Should().Be(SystemColors.HotTrackColor.A);
            (control.BorderBrush as SolidColorBrush).Color.A.Should().Be(SystemColors.HotTrackColor.A);
            control.BorderThickness.Should().Be(new Thickness(3));
        }
    }
}