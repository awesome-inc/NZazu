using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Fields;

namespace NZazu.Factory
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class NZazuFieldFactory_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuFieldFactory();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuFieldFactory>();
        }

        [Test]
        [TestCase(null)]
        [TestCase("label")]
        [TestCase("string")]
        [TestCase("bool")]
        public void Support(string fieldType)
        {
            var sut = new NZazuFieldFactory();

            var field = sut.CreateField(new FieldDefinition { Key = "test", Type = fieldType });

            field.Should().NotBeNull();
            field.Type.Should().Be(fieldType ?? "label"); // because of the fallback in case of null
        }

        [Test]
        public void Set_Checks()
        {
            var sut = new NZazuFieldFactory();
            var check = Substitute.For<IValueCheck>();
            var checks = new[] { check };
            var field = (NZazuField)sut.CreateField(new FieldDefinition { Key = "test", Type = "string", Checks = checks });

            field.Should().NotBeNull();
            field.Checks.Should().BeSameAs(checks);
        }
    }
}