using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Extensions;

namespace NZazu
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuView_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuView();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfView>();
        }

        [Test]
        public void Return_field_values_on_GetFieldValues()
        {
            var view = new NZazuView();

            const string key = "key";
            const string value = "value";

            var fieldDefinition = new FieldDefinition { Key = key };
            var formDefinition = new FormDefinition { Fields = new[] { fieldDefinition } };
            view.FormDefinition = formDefinition;

            var factory = Substitute.For<INZazuWpfFieldFactory>();
            var field = Substitute.For<INZazuWpfField>();
            field.StringValue = value;
            field.IsEditable.Returns(true);
            factory.CreateField(fieldDefinition).Returns(field);

            view.FieldFactory = factory;

            var actual = view.GetFieldValues();
            actual.Keys.Single().Should().Be(key);
            actual[key].Should().Be(value);
        }

        [Test]
        public void Recurse_on_group_fields_in_GetFieldValues()
        {
            var view = new NZazuView
            {
                FormDefinition = new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition {Key = "name", Type = "string"},
                        new FieldDefinition
                        {
                            Key = "group",
                            Type = "group",
                            Fields = new[]
                            {
                                new FieldDefinition {Key = "group.name", Type = "string"}
                            }
                        }
                    }
                }
            };

            var expected = new Dictionary<string, string>
            {
                { "name", "John" }, 
                { "group.name", "Jim" },
            };

            view.SetFieldValues(expected);

            var actual = view.GetFieldValues();
            actual.ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void Handle_FormData_Which_Has_More_Values_Than_Fields()
        {
            var sut = new NZazuView();
            sut.FormDefinition = new FormDefinition
            {
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = "name",
                        Type = "string",
                        Prompt = "Name",
                        Hint = "Enter name",
                        Description = "Your account name. Only alpha-numeric ..."
                    },
                    new FieldDefinition
                    {
                        Key = "isAdmin",
                        Type = "bool",
                        Hint = "Is Admin",
                        Description = "Check to grant administrator permissions"
                    }
                }
            };
            sut.FormData = new Dictionary<string, string>
            {
                {"name", "John"},
                {"isAdmin", "true"},
                {"iDontExist", "foo"}
            };
        }

        [Test]
        public void Update_when_FormDefinition_changed()
        {
            var formDefinition = new FormDefinition { Layout = "grid", Fields = new FieldDefinition[] { } };
            VerifyUpdate(view =>
            {
                view.FormDefinition = formDefinition;
                view.FormDefinition.Should().Be(formDefinition);
            });
        }

        [Test]
        public void Update_when_FieldFactory_changed()
        {
            var fieldFactory = Substitute.For<INZazuWpfFieldFactory>();
            VerifyUpdate(view =>
            {
                view.FieldFactory = fieldFactory;
                view.FieldFactory.Should().Be(fieldFactory);
            });
        }

        [Test]
        public void Update_when_LayoutStrategy_changed()
        {
            var layout = Substitute.For<IResolveLayout>();
            VerifyUpdate(view =>
            {
                view.ResolveLayout = layout;
                view.ResolveLayout.Should().Be(layout);
            });
        }

        static void VerifyUpdate(Action<INZazuWpfView> act)
        {
            var formDefinition = new FormDefinition { Fields = new FieldDefinition[] { } };

            var resolveLayout = Substitute.For<IResolveLayout>();
            var layout = Substitute.For<INZazuWpfLayoutStrategy>();
            resolveLayout.Resolve(formDefinition.Layout).Returns(layout);
            var fieldFactory = Substitute.For<INZazuWpfFieldFactory>();

            var sut = new NZazuView
            {
                ResolveLayout = resolveLayout,
                FieldFactory = fieldFactory,
                FormDefinition = formDefinition
            };

            fieldFactory.ClearReceivedCalls();
            resolveLayout.ClearReceivedCalls();
            layout.ClearReceivedCalls();

            act(sut);

            sut.ResolveLayout.Received().Resolve(sut.FormDefinition.Layout);
        }

        [Test]
        public void Disallow_null_FieldFactory()
        {
            var sut = new NZazuView();
            sut.FieldFactory.Should().NotBeNull();

            sut.FieldFactory = null;

            sut.FieldFactory.Should().NotBeNull();
        }

        [Test]
        public void Disallow_null_LayoutStrategy()
        {
            var sut = new NZazuView();
            sut.ResolveLayout.Should().NotBeNull();

            sut.ResolveLayout = null;

            sut.ResolveLayout.Should().NotBeNull();
        }

        [Test]
        public void Set_Field_Values_If_FormData_Changes()
        {
            var view = new NZazuView();

            const string key = "key";
            const string value = "value";

            var formDefinition = new FormDefinition { Fields = new[] { new FieldDefinition { Key = key, Type = "string" } } };
            view.FormDefinition = formDefinition;

            view.FormData = new Dictionary<string, string> { { key, value } };

            var actual = view.GetFieldValues();
            actual.Keys.Single().Should().Be(key);
            actual[key].Should().Be(value);
        }

        [Test]
        public void Update_FormData_On_LostFocus()
        {
            var view = new NZazuView();

            const string key = "key";
            const string value = "value";

            var formDefinition = new FormDefinition { Fields = new[] { new FieldDefinition { Key = key, Type = "string" } } };
            view.FormDefinition = formDefinition;
            var values = new Dictionary<string, string> { { key, value } };
            view.FormData = values;
            view.FormData.Values.ShouldBeEquivalentTo(values);

            // simulate user editing
            const string changedValue = "other";
            view.GetField(key).StringValue = changedValue;

            // simulate user leaves the field -> LostFoucs
            view.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));

            // verify (bound) FormData has been updated, so thumbs up for binding experience
            view.FormData.Values[key].Should().Be(changedValue);
        }

        [Test]
        public void Validate_By_Calling_INZazuField_Validate()
        {
            var field = Substitute.For<INZazuWpfField>();
            field.Key.ReturnsForAnyArgs("test");
            field.Validate().Returns(ValueCheckResult.Success);
            var fieldFactory = Substitute.For<INZazuWpfFieldFactory>();
            var layout = Substitute.For<IResolveLayout>();
            fieldFactory.CreateField(Arg.Any<FieldDefinition>()).Returns(field);

            var sut = new NZazuView
            {
                FormDefinition = new FormDefinition { Fields = new[] { new FieldDefinition { Key = "test" } } },
                FieldFactory = fieldFactory,
                ResolveLayout = layout
            };

            sut.Validate();
            field.ReceivedWithAnyArgs().Validate();
        }

        [Test]
        [NUnit.Framework.Description("In real-time scenarios try to preserve formdata when formdefinition changed only marginally")]
        public void Preserve_Formdata_if_FormDefinition_changed()
        {
            const string key = "name";
            const string value = "John";

            var fieldDefinition = new FieldDefinition { Key = key, Type = "string", Prompt = "Name" };
            var initialFormDefinition = new FormDefinition { Fields = new[] { fieldDefinition } };
            var formData = new FormData(new Dictionary<string, string> { { key, value } });

            var sut = new NZazuView { FormDefinition = initialFormDefinition, FormData = formData };

            sut.FormData.Should().Be(formData);
            var actual = sut.GetFieldValues();
            formData.Values.ShouldBeEquivalentTo(actual);

            fieldDefinition.Prompt = "Login";
            var changedFormDefinition = new FormDefinition {Fields = new[] {fieldDefinition}};

            // it is ugly to attach to depProperty.changed. It goes like this
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/262df30c-8383-4d5c-8d76-7b8e2cea51de/how-do-you-attach-a-change-event-to-a-dependency-property?forum=wpf
            var dpDescriptor = DependencyPropertyDescriptor.FromProperty(NZazuView.FormDefinitionProperty, typeof(NZazuView));
            var formDefinitionChanged = false;
            EventHandler handler = (sender, args) => { formDefinitionChanged = true; };
            dpDescriptor.AddValueChanged(sut, handler);
            try { sut.FormDefinition = changedFormDefinition; }
            finally { dpDescriptor.RemoveValueChanged(sut, handler); }            

            formDefinitionChanged.Should().BeTrue();

            actual = sut.GetFieldValues();
            actual.ShouldBeEquivalentTo(formData.Values);
        }


        [Test]
        [NUnit.Framework.Description("In real-time scenarios try to preserve formdat when formdefinition changed only marginally")]
        public void Throw_KeyNotFoundException_On_GetField_For_Wrong_Key()
        {
            const string key = "key";
            const string value = "value";

            var fieldDefinition = new FieldDefinition { Key = key, Type = "string", Prompt = "Name" };
            var formDefinition = new FormDefinition { Fields = new[] { fieldDefinition } };
            var formData = new FormData(new Dictionary<string, string> { { key, value } });

            var sut = new NZazuView { FormDefinition = formDefinition, FormData = formData };
            new Action(() => sut.GetField("I do not exist")).Invoking(a => a()).ShouldThrow<KeyNotFoundException>();

        }

        [Test]
        public void Attach_And_Detach_Behavior_To_Field()
        {
            const string key = "key";
            const string value = "value";

            // lets mock the behavior
            var behaviorDefinition = new BehaviorDefinition { Name = "Empty" };
            var behavior = Substitute.For<INZazuWpfFieldBehavior>();
            var fieldDefinition = new FieldDefinition
            {
                Key = key,
                Type = "string",
                Prompt = "Name",
                Behavior = behaviorDefinition
            };
            var formDefinition = new FormDefinition { Fields = new[] { fieldDefinition } };
            var formData = new FormData(new Dictionary<string, string> { { key, value } });
            var behaviorFactory = Substitute.For<INZazuWpfFieldBehaviorFactory>();
            behaviorFactory.CreateFieldBehavior(Arg.Any<BehaviorDefinition>()).ReturnsForAnyArgs(behavior);

            // amke sure an attach happens
            var sut = new NZazuView { FieldBehaviorFactory = behaviorFactory, FormDefinition = formDefinition, FormData = formData };
            sut.Should().NotBeNull();

            behavior.ReceivedWithAnyArgs().AttachTo(Arg.Any<INZazuWpfField>());
            behavior.ClearReceivedCalls();

            // now lets create a ner form and detach the existing behavior
            sut.FormDefinition = new FormDefinition { Fields = new[] { fieldDefinition } };
            behavior.ReceivedWithAnyArgs().Detach();
        }

        [Test]
        public void Skip_skip_fixed_fields_in_GetFieldValues()
        {
            var view = new NZazuView
            {
                FormDefinition = new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition {Key = "caption", Type = "label"},
                        new FieldDefinition {Key = "name", Type = "string"}
                    }
                }
            };

            var expected = new Dictionary<string, string> { { "name", "John" } };

            view.SetFieldValues(expected);

            var actual = view.GetFieldValues();
            actual.ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void Focus_specified_field_after_changing_FormDefinition()
        {
            var view = new NZazuView
            {
                FormDefinition = new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition {Key = "other", Type = "string"},
                        new FieldDefinition {Key = "focus", Type = "string"}
                    },
                    FocusOn = "focus"
                }
            };

            var otherCtrl = view.GetField("other").ValueControl;
            var keyCtrl = view.GetField("focus").ValueControl;

            otherCtrl.IsFocused.Should().BeFalse();
            keyCtrl.IsFocused.Should().BeTrue();

            view.TrySetFocusOn("other").Should().BeTrue();

            keyCtrl.IsFocused.Should().BeFalse();
            otherCtrl.IsFocused.Should().BeTrue();
        }
    }
}
