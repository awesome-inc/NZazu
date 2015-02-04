using System;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class ValidationException_Should
    {
        [Test]
        public void Be_Creatable()
        {
            const string message = "foo there";
            var sut = new ValidationException(message);

            sut.Should().NotBeNull();
            sut.Message.Should().Be(message);
        }

        [Test]
        public void Be_Creatable_With_Exception()
        {
            const string message = "foo there";
            var exception = new Exception();
            var sut = new ValidationException(message, exception);

            sut.Should().NotBeNull();
            sut.Message.Should().Be(message);
            sut.InnerException.Should().Be(exception);
        }
    }
}