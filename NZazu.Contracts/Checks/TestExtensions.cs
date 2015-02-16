using System;
using FluentAssertions;

namespace NZazu.Contracts.Checks
{
    static class TestExtensions
    {
        public static void ShouldFailWith<TError>(this IValueCheck check, string value, Predicate<TError> matchError = null)
        {
            var vr = check.Validate(value);
            vr.IsValid.Should().BeFalse();
            var error = (TError)vr.Error;
            error.Should().NotBeNull();
            if (matchError != null)
                matchError(error).Should().BeTrue();
        }

        public static void ShouldPass(this IValueCheck check, string value)
        {
            var vr = check.Validate(value);
            vr.IsValid.Should().BeTrue();
            vr.Error.Should().BeNull();
        }
    }
}