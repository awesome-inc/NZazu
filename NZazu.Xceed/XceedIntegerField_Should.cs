using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixtureFor(typeof (XceedIntegerField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedIntegerField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new XceedIntegerField(new FieldDefinition {Key="test"});

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
            sut.Type.Should().Be("int");
        }

        [Test]
        [STAThread]
        public void Use_IntegerUpdown()
        {
            var sut = new XceedIntegerField(new FieldDefinition {Key="test"});
            sut.ContentProperty.Should().Be(IntegerUpDown.ValueProperty);

            var control = (IntegerUpDown) sut.ValueControl;
            control.Should().NotBeNull();
        }

        [Test, Description("FormatString is limited on IntegerUpDown, unsupported format strings results in conversion errors")]
        // cf.: http://wpftoolkit.codeplex.com/wikipage?title=IntegerUpDown&referringTitle=NumericUpDown-derived%20controls#formatstring
        [TestCase("C0", "C0")]
        [TestCase("F2", "F2")]
        [TestCase("G", "G")]
        [TestCase("N2", "N2")]
        [TestCase("P3", "P3")]
        [TestCase("foobar", "")]
        [STAThread]
        public void Only_use_supported_FormatStrings(string formatString, string expected)
        {

            var sut = new XceedIntegerField(new FieldDefinition {Key="test"});
            sut.Settings.Add("Format", formatString);

            var control = (IntegerUpDown)sut.ValueControl;
            control.FormatString.Should().Be(expected);
        }
    }
}