using System;
using FluentAssertions;
using NEdifis.Attributes;

namespace NZazu.Contracts.Checks
{
    [ExcludeFromConventions("testing helper")]
    internal static class TestExtensions
    {
        public static void ShouldFailWith<TError>(this IValueCheck check, string value, Predicate<TError> matchError = null)
        {
            var vr = check.Validate(value);
            vr.IsValid.Should().BeFalse();
            var error = (TError)vr.Error;
            error.Should().NotBeNull();
            matchError?.Invoke(error).Should().BeTrue();
        }

        public static void ShouldPass(this IValueCheck check, string value)
        {
            var vr = check.Validate(value);
            vr.IsValid.Should().BeTrue();
            vr.Error.Should().BeNull();
        }
    }
}