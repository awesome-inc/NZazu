using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuField_Should
    {
        #region Test fields to verify base class

        [ExcludeFromCodeCoverage]
        private class NZazuDummyField : NZazuField
        {
            public NZazuDummyField(FieldDefinition definition) : base(definition) { }

            public override bool IsEditable { get { throw new NotImplementedException(); } }
            public override string StringValue { get; set; }
            public override string Type => null;
            public override DependencyProperty ContentProperty => null;
            protected internal override Control Value
            {
                get
                { return null; }
            }
        }

        [ExcludeFromCodeCoverage]
        private class NZazuField_With_Description_As_Content_Property : NZazuDummyField
        {
            public NZazuField_With_Description_As_Content_Property(FieldDefinition definition) : base(definition) { }

            public override DependencyProperty ContentProperty => ContentControl.ContentProperty;
            protected internal override Control Value
            {
                get
                { return new ContentControl(); }
            }
        }


        [ExcludeFromCodeCoverage]
        private class GenericDummyField : NZazuField<int>
        {
            public GenericDummyField(FieldDefinition definition) : base(definition) { }
            public override DependencyProperty ContentProperty { get { throw new NotImplementedException(); } }
            public override string Type { get { throw new NotImplementedException(); } }
            protected internal override Control Value
            {
                get
                { throw new NotImplementedException(); }
            }

            protected override void SetStringValue(string value) { throw new NotImplementedException(); }
            protected override string GetStringValue() { throw new NotImplementedException(); }
        }
        #endregion

        [Test]
        public void Validate_ctor_parameters()
        {
            // ReSharper disable ObjectCreationAsStatement
            0.Invoking(x => new NZazuDummyField(new FieldDefinition { Key = "" })).ShouldThrow<ArgumentException>();
            1.Invoking(x => new NZazuDummyField(new FieldDefinition())).ShouldThrow<ArgumentException>();
            2.Invoking(x => new NZazuDummyField(new FieldDefinition { Key = "\t\r\n "})).ShouldThrow<ArgumentException>();
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Not_Create_Label_if_no_prompt()
        {
            var sut = new NZazuDummyField(new FieldDefinition { Key = "test" });
            sut.Prompt.Should().BeNullOrWhiteSpace();
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        [STAThread]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuDummyField(new FieldDefinition {Key="test"})
            {
                Prompt = "superhero"
            };

            var label = (Label)sut.LabelControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Prompt);
        }

        [Test]
        public void Set_And_Get_Value()
        {
            var sut = new NZazuDummyField(new FieldDefinition {Key="test"});
            sut.StringValue.Should().BeNull();

            sut.StringValue = "test";

            sut.StringValue.Should().Be("test");
        }

        [Test]
        public void Pass_Validation_To_Checks()
        {
            var check = Substitute.For<IValueCheck>();
            var sut = new NZazuDummyField(new FieldDefinition {Key="test"}) { Description = "description", Check = check };
            sut.Validate();

            check.ReceivedWithAnyArgs().Validate(Arg.Any<string>());
        }

        [Test]
        public void Pass_Validation_To_Checks_and_returns_first_error_if_any()
        {
            var check = Substitute.For<IValueCheck>();
            check.Validate(Arg.Any<string>(), Arg.Any<IFormatProvider>()).Returns(new ValueCheckResult(false, "test"));

            var sut = new NZazuDummyField(new FieldDefinition {Key="test"}) { Description = "description", Check = check };
            sut.Validate().IsValid.Should().BeFalse();
            check.ReceivedWithAnyArgs().Validate(Arg.Any<string>());
        }

        [Test, Description("This test verifies that \"Settings\" cannot become null")]
        public void Have_Settings_not_null()
        {
            var field = new NZazuDummyField(new FieldDefinition { Key = "key" });
            field.Settings.Should().NotBeNull();

            var propInfo = typeof(NZazuField).GetProperty(nameof(NZazuField.Settings));
            propInfo.GetSetMethod(true).Should().BeNull();
        }

        [Test]
        [STAThread]
        public void Respect_Height_Setting()
        {
            var field = new NZazuField_With_Description_As_Content_Property(new FieldDefinition { Key = "key" });
            const double expected = 65.5;
            field.Settings.Add("Height", expected.ToString(CultureInfo.InvariantCulture));

            var control = (ContentControl)field.ValueControl;
            control.MinHeight.Should().Be(expected);
            control.MaxHeight.Should().Be(expected);
        }

        [Test]
        [STAThread]
        public void Respect_Width_Setting()
        {
            var field = new NZazuField_With_Description_As_Content_Property(new FieldDefinition { Key = "key" });
            const double expected = 65.5;
            field.Settings.Add("Width", expected.ToString(CultureInfo.InvariantCulture));

            var control = (ContentControl)field.ValueControl;
            control.MinWidth.Should().Be(expected);
            control.MaxWidth.Should().Be(expected);
        }

        [Test]
        [SetCulture("fr")]
        public void Use_InvariantCulture_in_GetSettingT()
        {
            var field = new NZazuDummyField(new FieldDefinition { Key = "key" });
            const double expectedHeight = 65.5;
            field.Settings.Add("Height", expectedHeight.ToString(CultureInfo.InvariantCulture));

            field.GetSetting<double>("Height").Should().Be(expectedHeight);
        }

        #region test NZazuDummyField with bi-directional content property

        [Test]
        [STAThread]
        public void Attach_FieldValidationRule_according_to_checks()
        {
            // but we need a dummy content enabled field -> no content, no validation
            var check = Substitute.For<IValueCheck>();
            check.Validate(Arg.Any<string>()).Returns(new ValueCheckResult(false, "test"));

            var sut = new NZazuField_With_Description_As_Content_Property(new FieldDefinition {Key="test"})
            { Description = "description", Check = check };

            var expectedRule = new CheckValidationRule(check)
            {
                // we make sure the validation is executed on init
                ValidatesOnTargetUpdated = true
            };
            sut.ValueControl.Should().NotBeNull();

            var expression = sut.ValueControl.GetBindingExpression(sut.ContentProperty);
            expression.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
            var binding = expression.ParentBinding;
            binding.Should().NotBeNull();
            binding.Source.Should().Be(sut, because: "we need a source for data binding");
            binding.Mode.Should().Be(BindingMode.TwoWay, because: "we update databinding in two directions");
            binding.UpdateSourceTrigger
                .Should().Be(UpdateSourceTrigger.PropertyChanged, because: "we want validation during edit");

            binding.ValidationRules.Single().ShouldBeEquivalentTo(expectedRule);
        }

        #endregion

        [Test]
        public void Be_Editable()
        {
            var sut = new GenericDummyField(new FieldDefinition {Key="test"});
            sut.IsEditable.Should().BeTrue();
        }

        [Test]
        [STAThread]
        public void Respect_generic_settings()
        {
            var sut = new NZazuField_With_Description_As_Content_Property(new FieldDefinition {Key="test"});

            sut.Settings.Add("ContentStringFormat", "dddd – d - MMMM");
            sut.Settings.Add("FontFamily", "Century Gothic");
            sut.Settings.Add("FontWeight", "UltraBold");
            sut.Settings.Add("FontSize", "24");
            sut.Settings.Add("Foreground", "BlueViolet");
            sut.Settings.Add("Margin", "1,2,3,4");
            sut.Settings.Add("Name", "myControl");
            sut.Settings.Add("Opacity", "0.75");
            sut.Settings.Add("Padding", "2.5");
            sut.Settings.Add("TabIndex", "42");
            sut.Settings.Add("Uid", "myId");
            sut.Settings.Add("Visibility", "Collapsed");
            sut.Settings.Add("HorizontalAlignment", "Left");
            sut.Settings.Add("VerticalAlignment", "Bottom");

            var control = (ContentControl)sut.ValueControl;

            control.FontFamily.ToString().Should().Be("Century Gothic");
            control.FontWeight.Should().Be(FontWeights.UltraBold);
            control.FontSize.Should().Be(24);

            var brush = (SolidColorBrush)control.Foreground;
            brush.Color.Should().Be(Colors.BlueViolet);

            control.Margin.Should().Be(new Thickness(1, 2, 3, 4));
            control.Name.Should().Be("myControl");
            control.Opacity.Should().Be(0.75);
            control.Padding.Should().Be(new Thickness(2.5));
            control.ContentStringFormat.Should().Be("dddd – d - MMMM");
            control.TabIndex.Should().Be(42);
            control.Uid.Should().Be("myId");
            control.Visibility.Should().Be(Visibility.Collapsed);

            control.HorizontalAlignment.Should().Be(HorizontalAlignment.Left);
            control.VerticalAlignment.Should().Be(VerticalAlignment.Bottom);
        }
    }
}