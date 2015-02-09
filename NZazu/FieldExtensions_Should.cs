using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts.Checks;

namespace NZazu
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    internal class FieldExtensions_Should
    {
        [Test]
        public void Return_False_If_Validate_Has_Exception()
        {
            var field = Substitute.For<INZazuWpfField>();
            field.WhenForAnyArgs(f => f.Validate()).Do(info => { throw new ValidationException("I am invalid"); });

            field.IsValid().Should().BeFalse();

            field.ReceivedWithAnyArgs().Validate();
        }

        [Test]
        public void Return_True_If_Validate()
        {
            var field = Substitute.For<INZazuWpfField>();

            field.IsValid().Should().BeTrue();

            field.ReceivedWithAnyArgs().Validate();
        }
    }
}