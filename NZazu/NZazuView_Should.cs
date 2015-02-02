using FluentAssertions;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class NZazuView_Should
    {
        [Test]
        [RequiresSTA]
        public void Be_Creatable()
        {
            var sut = new NZazuView();

            sut.Should().NotBeNull();
        }

        [Test]
        [RequiresSTA]
        [TestCase("userName", "string", "User", "Enter user name", "The user's name")]
        [TestCase("gender", "bool", "Gender", "Choose gender", "The user's gender")]
        // ReSharper disable once TooManyArguments
        public void Support_field(string key, string type, string prompt, string placeholder, string description)
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
        }
    }
}
