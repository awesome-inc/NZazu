using System;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixtureFor(typeof(DateTimePickerWithUpdate))]
    // ReSharper disable once InconsistentNaming
    internal class DateTimePickerWithUpdate_Should
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Be_Creatable()
        {
            var sut = new DateTimePickerWithUpdate();

            sut.Should().NotBeNull();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Change_Value()
        {
            var bd = new DateTime(1980, 2, 20, 14, 23, 12);
            var sut = new DateTimePickerWithUpdate();
            var field = sut.GetType().GetField("_valuePicker", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            // ReSharper disable once PossibleNullReferenceException
            var ctrl = (DateTimePicker)field.GetValue(sut);

            // lest test...
            sut.Value = bd;
            sut.Value.Should().BeCloseTo(bd);
            ctrl.Value.Should().BeCloseTo(bd);

            // now press the button
            sut.UpdateToToday_OnClick(this, null);

            sut.Value.Should().BeCloseTo(DateTime.Now, 2000);
            ctrl.Value.Should().BeCloseTo(DateTime.Now, 2000);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Use_Format_Settings()
        {
            var sut = new DateTimePickerWithUpdate()
            {
                Watermark = "the watermark",
                Format = DateTimeFormat.Custom,
                FormatString = "yyyy-MMM-dd"
            };
            sut.Watermark.Should().Be("the watermark");
            sut.Format.Should().Be(DateTimeFormat.Custom);
            sut.FormatString.Should().Be("yyyy-MMM-dd");

            var field = sut.GetType().GetField("_valuePicker", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            // ReSharper disable once PossibleNullReferenceException
            var ctrl = (DateTimePicker)field.GetValue(sut);

            ctrl.Watermark.Should().Be("the watermark");
            ctrl.Format.Should().Be(DateTimeFormat.Custom);
            ctrl.FormatString.Should().Be("yyyy-MMM-dd");
        }

    }
}