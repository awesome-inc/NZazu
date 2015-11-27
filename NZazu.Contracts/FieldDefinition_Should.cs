using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    internal class FieldDefinition_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new FieldDefinition();

            sut.Should().NotBeNull();
        }
        
        [Test]
        public void Have_Settings_For_Additional_Confi()
        {
            var sut = new FieldDefinition();

            sut.Should().NotBeNull();
            sut.Settings.Should().BeNull();
        }

        [Test]
        public void Have_Behavior()
        {
            var sut = new FieldDefinition();
            sut.Behavior.Should().BeNull();
        }
    }
}