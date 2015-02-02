using System;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using NZazu.Contracts;

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
        }

        [Test]
        [TestCase("heading", "label", "Settings", null, "You can manage your account here. Use TAB ...", typeof(Label))]
        [TestCase("userName", "string", "User", "Enter your name", "Your name", typeof(TextBox))]
        [TestCase("gender", "bool", "Gender", "Choose gender", "Your gender", typeof (CheckBox))]
        //[TestCase("birthday", "dateTime", "Birthday", "Choose birthday", "Your birthday", typeof (DatePicker))]
        //[TestCase("weight", "double", "Weight", "Enter body weight (kg)", "Your weight", typeof(TextBox))]
        // ReSharper disable once TooManyArguments
        public void Support_field(string key, string type, string prompt, string placeholder, string description, Type controlType)
        {
            var sut = (INZazuView)new NZazuView();

            var formDefinition = new FormDefinition
            {
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = key, 
                        Type = type,
                        Prompt = prompt,
                        Placeholder = placeholder,
                        Description = description
                    }
                }
            };

            sut.FormDefinition = formDefinition;

            var field = sut.GetField(key);

            field.Should().NotBeNull();
            field.Key.Should().Be(key);
            field.Type.Should().Be(type);
            field.Prompt.Should().Be(prompt);
            field.Description.Should().Be(description);

            var control = field.Control;
            control.Should().BeOfType(controlType);
        }
    }
}
