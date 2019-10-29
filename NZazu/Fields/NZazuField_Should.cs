using System;
using System.Collections.Generic;
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
using NZazu.Extensions;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuField_Should
    {
        [ExcludeFromCodeCoverage]
        private object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        public void Validate_ctor_parameters()
        {
            // ReSharper disable ObjectCreationAsStatement
            0.Invoking(x => new NZazuDummyField(new FieldDefinition {Key = ""}, ServiceLocator)).Should()
                .Throw<ArgumentException>();
            1.Invoking(x => new NZazuDummyField(new FieldDefinition(), ServiceLocator)).Should()
                .Throw<ArgumentException>();
            2.Invoking(x => new NZazuDummyField(new FieldDefinition {Key = "\t\r\n "}, ServiceLocator)).Should()
                .Throw<ArgumentException>();
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Not_Create_Label_if_no_prompt()
        {
            var sut = new NZazuDummyField(new FieldDefinition {Key = "test"}, ServiceLocator);
            sut.Definition.Prompt.Should().BeNullOrWhiteSpace();
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        [STAThread]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuDummyField(new FieldDefinition {Key = "test", Prompt = "superhero"}, ServiceLocator);

            var label = (Label) sut.LabelControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Definition.Prompt);
        }

        [Test]
        public void Set_And_Get_Value()
        {
            var sut = new NZazuDummyField(new FieldDefinition {Key = "test"}, ServiceLocator);
            sut.GetValue().Should().BeNull();

            sut.SetValue("test");
            sut.GetValue().Should().Be("test");
        }

        [Test]
        public void Pass_Validation_To_Checks()
        {
            var check = Substitute.For<IValueCheck>();
            var sut = new NZazuDummyField(new FieldDefinition {Key = "test", Description = "description"},
                ServiceLocator) {Check = check};
            var result = sut.Validate();

            check.ReceivedWithAnyArgs().Validate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IFormatProvider>());
        }

        [Test]
        public void Pass_Validation_To_Checks_and_returns_first_error_if_any()
        {
            var check = Substitute.For<IValueCheck>();
            check.Validate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IFormatProvider>())
                .Returns(new ValueCheckResult(false, new Exception("test")));

            var sut = new NZazuDummyField(new FieldDefinition {Key = "test", Description = "description"},
                ServiceLocator) {Check = check};
            sut.Validate().IsValid.Should().BeFalse();
            check.ReceivedWithAnyArgs().Validate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IFormatProvider>());
        }

        [Test]
        [STAThread]
        public void Respect_Height_Setting()
        {
            const double expected = 65.5;
            var definition = new FieldDefinition {Key = "key"};
            definition.Settings.Add("Height", expected.ToString(CultureInfo.InvariantCulture));

            var field = new NZazuField_With_Description_As_Content_Property(definition, ServiceLocator);
            field.ApplySettings(definition);

            var control = (ContentControl) field.ValueControl;
            control.MinHeight.Should().Be(expected);
            control.MaxHeight.Should().Be(expected);
        }

        [Test]
        [STAThread]
        public void Respect_Width_Setting()
        {
            const double expected = 65.5;
            var definition = new FieldDefinition {Key = "key"};
            definition.Settings.Add("Width", expected.ToString(CultureInfo.InvariantCulture));

            var field = new NZazuField_With_Description_As_Content_Property(definition, ServiceLocator);
            field.ApplySettings(definition);

            var control = (ContentControl) field.ValueControl;
            control.MinWidth.Should().Be(expected);
            control.MaxWidth.Should().Be(expected);
        }

        [Test]
        [SetCulture("fr")]
        public void Use_InvariantCulture_in_GetSettingT()
        {
            var field = new NZazuDummyField(new FieldDefinition {Key = "key"}, ServiceLocator);
            const double expectedHeight = 65.5;
            field.Definition.Settings.Add("Height", expectedHeight.ToString(CultureInfo.InvariantCulture));

            field.Definition.Settings.Get<double>("Height").Should().Be(expectedHeight);
        }

        #region test NZazuDummyField with bi-directional content property

        [Test]
        [STAThread]
        public void Attach_FieldValidationRule_according_to_checks()
        {
            // but we need a dummy content enabled field -> no content, no validation
            var check = Substitute.For<IValueCheck>();
            check.Validate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CultureInfo>())
                .Returns(new ValueCheckResult(false, new Exception("test")));

            var sut = new NZazuField_With_Description_As_Content_Property(
                    new FieldDefinition {Key = "test", Description = "description"}, ServiceLocator)
                {Check = check};

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
            binding.Source.Should().Be(sut, "we need a source for data binding");
            binding.Mode.Should().Be(BindingMode.TwoWay, "we update data binding in two directions");
            binding.UpdateSourceTrigger
                .Should().Be(UpdateSourceTrigger.PropertyChanged, "we want validation during edit");

            binding.ValidationRules.Single().Should().BeEquivalentTo(expectedRule);
        }

        #endregion

        [Test]
        public void Be_Editable()
        {
            var sut = new GenericDummyField(new FieldDefinition {Key = "test"}, ServiceLocator);
            sut.IsEditable.Should().BeTrue();
        }

        [Test]
        [STAThread]
        public void Respect_generic_settings()
        {
            var definition = new FieldDefinition {Key = "test"};
            definition.Settings.Add("ContentStringFormat", "dddd – d - MMMM");
            definition.Settings.Add("FontFamily", "Century Gothic");
            definition.Settings.Add("FontWeight", "UltraBold");
            definition.Settings.Add("FontSize", "24");
            definition.Settings.Add("Foreground", "BlueViolet");
            definition.Settings.Add("Margin", "1,2,3,4");
            definition.Settings.Add("Name", "myControl");
            definition.Settings.Add("Opacity", "0.75");
            definition.Settings.Add("Padding", "2.5");
            definition.Settings.Add("TabIndex", "42");
            definition.Settings.Add("Uid", "myId");
            definition.Settings.Add("Visibility", "Collapsed");
            definition.Settings.Add("HorizontalAlignment", "Left");
            definition.Settings.Add("VerticalAlignment", "Bottom");

            var sut = new NZazuField_With_Description_As_Content_Property(definition, ServiceLocator);
            sut.ApplySettings(definition);
            var control = (ContentControl) sut.ValueControl;

            control.FontFamily.ToString().Should().Be("Century Gothic");
            control.FontWeight.Should().Be(FontWeights.UltraBold);
            control.FontSize.Should().Be(24);

            var brush = (SolidColorBrush) control.Foreground;
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

        [Test]
        public void Define_Multiple_NullReference_Behaviours()
        {
            var fieldDefinition = new FieldDefinition {Key = "test"};
            BehaviorDefinition behavior1 = null;
            BehaviorDefinition behavior2 = null;
            BehaviorDefinition behavior3 = null;

            fieldDefinition.Behaviors = new List<BehaviorDefinition> {behavior1, behavior2, behavior3};
            var sut = new GenericDummyField(fieldDefinition, ServiceLocator);

            Assert.DoesNotThrow(() => sut.Dispose());
        }

        [Test]
        [TestCase("Width")]
        [TestCase("Height")]
        [TestCase("Format")]
        public void Handle_Empty_Property_Values(string propertyName)
        {
            var fieldDefinition = new FieldDefinition {Key = "test"};

            fieldDefinition.Settings.Add(propertyName, "");
            var field = new NZazuField_With_Description_As_Content_Property(fieldDefinition, ServiceLocator);

            Assert.DoesNotThrow(() => field.ApplySettings(fieldDefinition));
        }

        #region Test fields to verify base class

        [ExcludeFromCodeCoverage]
        private class NZazuDummyField : NZazuField<string>
        {
            private string _stringValue;

            public NZazuDummyField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
                : base(definition, serviceLocatorFunc)
            {
            }

            public override DependencyProperty ContentProperty => null;

            public override void SetValue(string value)
            {
                _stringValue = value;
            }

            public override string GetValue()
            {
                return _stringValue;
            }

            protected override Control CreateValueControl()
            {
                return null;
            }
        }

        [ExcludeFromCodeCoverage]
        private class NZazuField_With_Description_As_Content_Property : NZazuDummyField
        {
            public NZazuField_With_Description_As_Content_Property(FieldDefinition definition,
                Func<Type, object> serviceLocatorFunc)
                : base(definition, serviceLocatorFunc)
            {
            }

            public override DependencyProperty ContentProperty => ContentControl.ContentProperty;

            protected override Control CreateValueControl()
            {
                return new ContentControl();
            }
        }


        [ExcludeFromCodeCoverage]
        private class GenericDummyField : NZazuField<int>
        {
            public GenericDummyField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
                : base(definition, serviceLocatorFunc)
            {
            }

            public override DependencyProperty ContentProperty => throw new NotImplementedException();

            protected override Control CreateValueControl()
            {
                throw new NotImplementedException();
            }

            public override void SetValue(string value)
            {
                throw new NotImplementedException();
            }

            public override string GetValue()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}