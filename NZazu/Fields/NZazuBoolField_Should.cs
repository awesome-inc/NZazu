using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuBoolField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuBoolField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuField>();
        }

        [Test]
        public void Not_Create_Empty_Label()
        {
            var sut = new NZazuBoolField("test");
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuBoolField("test")
            {
                Prompt = "superhero"
            };

            var label = (Label)sut.LabelControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Prompt);
        }
    }
}