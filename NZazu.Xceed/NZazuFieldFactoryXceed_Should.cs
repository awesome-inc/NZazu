using FluentAssertions;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu.Xceed
{
    [TestFixture, RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuFieldFactoryXceed_Should
    {
        [Test]
        public void Use_Watermarktextbox()
        {
            var sut = new NZazuFieldFactoryXceed();
            var fieldDefinition = new FieldDefinition
            {
                Key = "login",
                Type = "string",
                Prompt = "login",
                Hint = "Enter user name",
                Description = "Your User name"
            };
            var field = sut.CreateField(fieldDefinition);
            field.Should().BeOfType<NZazuWatermarkTextBoxField>();
        }
    }
}