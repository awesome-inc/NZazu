using System;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using NZazu;
using NZazu.Contracts;

namespace Sample
{
    [TestFixture, RequiresSTA, Explicit]
    // ReSharper disable InconsistentNaming
    class NZazu_Should
    {
        [Test]
        [TestCase("fallback", null, "Heading", null, "fallback text")]
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

        }
    }
}