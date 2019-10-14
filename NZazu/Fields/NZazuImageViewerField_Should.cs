using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuImageViewerField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable once InconsistentNaming
    internal class NZazuImageViewerField_Should
    {
        [ExcludeFromCodeCoverage]
        private object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        //[Ignore("skip for now")]
        public void Be_Creatable()
        {
            var sut = new NZazuImageViewerField(new FieldDefinition {Key = "test", Type = "imageViewer"},
                ServiceLocator);

            sut.Key.Should().Be("test");
            sut.ValueControl.Should().BeAssignableTo<ContentControl>();
            sut.IsEditable.Should().BeTrue();

            sut.Definition.Settings.Add("AllowCustomValues", "true"); // so heise.de works

            const string value = "http://heise.de";
            sut.SetValue(value);
            sut.GetValue().Should().Be(value);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        [TestCase(null, null)]
        [TestCase("foo bar", null)]
        [TestCase(@"http://img/4.jpg", null)] // because custom values not allowed
        [TestCase(@"http://img/3.jpg", @"http://img/3.jpg")] // because after toggle it starts at index 0
        public void Toggle_Values_Without_Custom_Values(string initValue, string toggleStartValue)
        {
            var sut = new NZazuImageViewerField(new FieldDefinition
            {
                Key = "test",
                Type = "imageViewer",
                Values = new[] {@"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg"}
            }, ServiceLocator);
            sut.SetValue(initValue);

            sut.GetValue().Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/2.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/3.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/1.jpg");
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        [TestCase(null, null)]
        [TestCase("foo bar", null)]
        [TestCase(@"http://img/4.jpg", null)] // because custom values not allowed
        [TestCase(@"http://img/3.jpg", @"http://img/3.jpg")] // because after toggle it starts at index 0
        public void Toggle_Back_Values_Without_Custom_Values(string initValue, string toggleStartValue)
        {
            var sut = new NZazuImageViewerField(new FieldDefinition
            {
                Key = "test",
                Type = "imageViewer",
                Values = new[] {@"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg"}
            }, ServiceLocator);
            sut.SetValue(initValue);

            sut.GetValue().Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/2.jpg");
            sut.ToggleValues(true);
            sut.GetValue().Should().Be(@"http://img/1.jpg");
            sut.ToggleValues(true);
            sut.GetValue().Should().Be(@"http://img/3.jpg");
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        [TestCase(null, null)]
        [TestCase("foo bar", null)]
        [TestCase(@"http://img/4.jpg", null)] // because custom values not allowed
        public void Toggle_Values_No_Custom_But_Nulls(string initValue, string toggleStartValue)
        {
            var sut = new NZazuImageViewerField(new FieldDefinition
            {
                Key = "test",
                Type = "imageViewer",
                Values = new[] {@"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg"}
            }, ServiceLocator);
            sut.Definition.Settings.Add("AllowNullValues", "true");
            sut.Definition.Settings.Add("AllowCustomValues", "false");
            sut.SetValue(initValue);

            sut.GetValue().Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/2.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/3.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(null);
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/1.jpg");
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        [TestCase(null, null)]
        [TestCase("foo bar", null)]
        [TestCase(@"http://img/4.jpg", null)] // because custom values not allowed
        public void ToggleBack_Values_No_Custom_But_Nulls(string initValue, string toggleStartValue)
        {
            var sut = new NZazuImageViewerField(new FieldDefinition
            {
                Key = "test",
                Type = "imageViewer",
                Values = new[] {@"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg"}
            }, ServiceLocator);
            sut.Definition.Settings.Add("AllowNullValues", "true");
            sut.Definition.Settings.Add("AllowCustomValues", "false");
            sut.SetValue(initValue);

            sut.GetValue().Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/2.jpg");
            sut.ToggleValues(true);
            sut.GetValue().Should().Be(@"http://img/1.jpg");
            sut.ToggleValues(true);
            sut.GetValue().Should().Be(null);
            sut.ToggleValues(true);
            sut.GetValue().Should().Be(@"http://img/3.jpg");
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        [TestCase(@"http://img/4.jpg", @"http://img/4.jpg")]
        public void Toggle_Values_With_Custom_Values(string initValue, string toggleStartValue)
        {
            var sut = new NZazuImageViewerField(new FieldDefinition
            {
                Key = "test",
                Type = "imageViewer",
                Values = new[] {@"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg"}
            }, ServiceLocator);
            sut.Definition.Settings.Add("AllowNullValues", "false");
            sut.Definition.Settings.Add("AllowCustomValues", "true");
            sut.SetValue(initValue);

            sut.GetValue().Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/2.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(@"http://img/3.jpg");
            sut.ToggleValues();
            sut.GetValue().Should().Be(toggleStartValue);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Toggle_EmptyValues_With_CustomValues()
        {
            var initValue = "http://img/4.jpg";
            var sut = new NZazuImageViewerField(new FieldDefinition
            {
                Key = "test",
                Type = "imageViewer"
            }, ServiceLocator);
            sut.Definition.Settings.Add("AllowNullValues", "false");
            sut.Definition.Settings.Add("AllowCustomValues", "true");
            sut.SetValue(initValue);

            sut.GetValue().Should().Be(initValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.GetValue().Should().Be(initValue);
            sut.ToggleValues();
            sut.GetValue().Should().Be(initValue);

            sut.Definition.Settings["AllowNullValues"] = "true";
            sut.ToggleValues();
            sut.GetValue().Should().Be(null);
            sut.ToggleValues();
            sut.GetValue().Should().Be(initValue);
            sut.ToggleValues();
            sut.GetValue().Should().Be(null);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Toggle_EmptyValues_Without_CustomValues()
        {
            var sut = new NZazuImageViewerField(new FieldDefinition
            {
                Key = "test",
                Type = "imageViewer"
            }, ServiceLocator);
            sut.Definition.Settings.Add("AllowNullValues", "false");
            sut.Definition.Settings.Add("AllowCustomValues", "false");
            sut.SetValue(null);

            sut.GetValue().Should().Be(null);

            // lets toggle a bit
            sut.ToggleValues();
            sut.GetValue().Should().Be(null);
            sut.ToggleValues();
            sut.GetValue().Should().Be(null);

            sut.Definition.Settings["AllowNullValues"] = "true";
            sut.ToggleValues();
            sut.GetValue().Should().Be(null);
            sut.ToggleValues();
            sut.GetValue().Should().Be(null);
            sut.ToggleValues();
            sut.GetValue().Should().Be(null);
        }
    }
}