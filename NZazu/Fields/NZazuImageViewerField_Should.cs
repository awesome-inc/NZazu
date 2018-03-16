using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuImageViewerField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable once InconsistentNaming
    internal class NZazuImageViewerField_Should
    {
        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        //[Ignore("skip for now")]
        public void Be_Creatable()
        {
            var sut = new NZazuImageViewerField(new FieldDefinition { Key = "test", Type = "imageViewer" });

            sut.Key.Should().Be("test");
            sut.ValueControl.Should().BeAssignableTo<ContentControl>();
            sut.Type.Should().Be("imageViewer");
            sut.IsEditable.Should().BeTrue();

            sut.Settings.Add("AllowCustomValues", "true"); // so heise.de works

            const string value = "http://heise.de";
            sut.StringValue = value;
            sut.StringValue.Should().Be(value);
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
                Values = new[] { @"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg" }
            })
            { StringValue = initValue };

            sut.StringValue.Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/2.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/3.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/1.jpg");
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
                Values = new[] { @"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg" }
            })
            { StringValue = initValue };

            sut.StringValue.Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/2.jpg");
            sut.ToggleValues(true);
            sut.StringValue.Should().Be(@"http://img/1.jpg");
            sut.ToggleValues(true);
            sut.StringValue.Should().Be(@"http://img/3.jpg");
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
                Values = new[] { @"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg" },
            });
            sut.Settings.Add("AllowNullValues", "true");
            sut.Settings.Add("AllowCustomValues", "false");
            sut.StringValue = initValue;

            sut.StringValue.Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/2.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/3.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(null);
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/1.jpg");
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
                Values = new[] { @"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg" },
            });
            sut.Settings.Add("AllowNullValues", "true");
            sut.Settings.Add("AllowCustomValues", "false");
            sut.StringValue = initValue;

            sut.StringValue.Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/2.jpg");
            sut.ToggleValues(true);
            sut.StringValue.Should().Be(@"http://img/1.jpg");
            sut.ToggleValues(true);
            sut.StringValue.Should().Be(null);
            sut.ToggleValues(true);
            sut.StringValue.Should().Be(@"http://img/3.jpg");
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
                Values = new[] { @"http://img/1.jpg", @"http://img/2.jpg", @"http://img/3.jpg" },
            });
            sut.Settings.Add("AllowNullValues", "false");
            sut.Settings.Add("AllowCustomValues", "true");
            sut.StringValue = initValue;

            sut.StringValue.Should().Be(toggleStartValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/1.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/2.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(@"http://img/3.jpg");
            sut.ToggleValues();
            sut.StringValue.Should().Be(toggleStartValue);
        }

        [Test]
        [STAThread]
        [Apartment(ApartmentState.STA)]
        public void Toggle_EmptyValues_With_CustomValues()
        {
            string initValue = "http://img/4.jpg";
            var sut = new NZazuImageViewerField(new FieldDefinition
            {
                Key = "test",
                Type = "imageViewer"
            });
            sut.Settings.Add("AllowNullValues", "false");
            sut.Settings.Add("AllowCustomValues", "true");
            sut.StringValue = initValue;

            sut.StringValue.Should().Be(initValue);

            // lets toggle a bit
            sut.ToggleValues();
            sut.StringValue.Should().Be(initValue);
            sut.ToggleValues();
            sut.StringValue.Should().Be(initValue);

            sut.Settings["AllowNullValues"] = "true";
            sut.ToggleValues();
            sut.StringValue.Should().Be(null);
            sut.ToggleValues();
            sut.StringValue.Should().Be(initValue);
            sut.ToggleValues();
            sut.StringValue.Should().Be(null);
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
            });
            sut.Settings.Add("AllowNullValues", "false");
            sut.Settings.Add("AllowCustomValues", "false");
            sut.StringValue = null;

            sut.StringValue.Should().Be(null);

            // lets toggle a bit
            sut.ToggleValues();
            sut.StringValue.Should().Be(null);
            sut.ToggleValues();
            sut.StringValue.Should().Be(null);

            sut.Settings["AllowNullValues"] = "true";
            sut.ToggleValues();
            sut.StringValue.Should().Be(null);
            sut.ToggleValues();
            sut.StringValue.Should().Be(null);
            sut.ToggleValues();
            sut.StringValue.Should().Be(null);
        }
    }
}