using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using NZazu;
using NZazu.Contracts;

namespace Sample
{
    [TestFixture, RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazu_Should
    {
        [Test]
        [TestCase("fallback", null, "Heading", null, "fallback text", typeof(Label))]
        [TestCase("heading", "label", "Settings", null, "You can manage your account here. Use TAB ...", typeof(Label))]
        [TestCase("userName", "string", "User", "Enter your name", "Your name", typeof(TextBox))]
        [TestCase("gender", "bool", "Admin", "Is Admin", "Check if you are an admin", typeof(CheckBox))]
        //[TestCase("birthday", "dateTime", "Birthday", "Choose birthday", "Your birthday", typeof (DatePicker))]
        //[TestCase("weight", "double", "Weight", "Enter body weight (kg)", "Your weight", typeof(TextBox))]
        // ReSharper disable once TooManyArguments
        public void Support_field(string key, string type, string prompt, string hint, string description, Type controlType)
        {
            var sut = new NZazuView();

            var formDefinition = new FormDefinition
            {
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = key, 
                        Type = type,
                        Prompt = prompt,
                        Hint = hint,
                        Description = description,
                    }
                }
            };

            sut.FormDefinition = formDefinition;

            var field = sut.GetField(key);

            field.Should().NotBeNull();
            field.Key.Should().Be(key);
            field.Type.Should().Be(type ?? "label"); // to make sure fallback is used
            field.Prompt.Should().Be(prompt);
            field.Hint.Should().Be(hint);
            field.Description.Should().Be(description);

            var control = field.ValueControl;
            control.Should().BeOfType(controlType);

            VerifyControl(sut, field);
        }

        private static void VerifyControl(NZazuView sut, INZazuField field)
        {
            var control = field.ValueControl;
            var child = FindChild(sut, ctrl => Equals(ctrl, control));
            child.Should().NotBeNull();
        }

        private static DependencyObject FindChild(DependencyObject parent, Predicate<DependencyObject> matches)
        {
            if (parent == null) return null;

            var children = LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>();
            foreach (var child in children)
            {
                if (matches(child))
                    return child;
                var foundChild = FindChild(child, matches);
                if (matches(foundChild))
                    return foundChild;
            }
            return null;
        }
    }
}