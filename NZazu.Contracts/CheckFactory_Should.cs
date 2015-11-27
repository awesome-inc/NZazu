using System;
using FluentAssertions;
using NUnit.Framework;
using NZazu.Contracts.Checks;

namespace NZazu.Contracts
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    internal class CheckFactory_Should
    {
        [Test]
        [TestCase("required", null, typeof(RequiredCheck))]
        [TestCase("length", new[]{"6", "8"}, typeof(StringLengthCheck))]
        [TestCase("range", new[] { "-42", "42" }, typeof(RangeCheck))]
        [TestCase("regex", new[] { "Must be true or false", "true", "false" }, typeof(StringRegExCheck))]
        public void Support(string type, string[] values, Type checkType)
        {
            var sut = new CheckFactory();

            var checkDefinition = new CheckDefinition {Type = type, Values = values};
            var check = sut.CreateCheck(checkDefinition);

            check.Should().NotBeNull();
            check.Should().BeOfType(checkType);
        }

        [Test]
        public void Throw_on_unsupported_types()
        {
            var sut = new CheckFactory();
            sut.Invoking(x => x.CreateCheck(null)).ShouldThrow<ArgumentNullException>();
            sut.Invoking(x => x.CreateCheck(new CheckDefinition())).ShouldThrow<ArgumentException>().WithMessage("check type not specified");
            sut.Invoking(x => x.CreateCheck(new CheckDefinition { Type = "foobar" })).ShouldThrow<NotSupportedException>().WithMessage("The specified check is not supported");
        }
        
        [Test]
        public void Support_Length_Check()
        {
            var sut = new CheckFactory();

            var checkDefinition = new CheckDefinition { Type = "length", Values = new []{"6","8"} };

            var check = sut.CreateCheck(checkDefinition);
            var actual = (StringLengthCheck) check;
            actual.MinimumLength.Should().Be(6);
            actual.MaximumLength.Should().Be(8);

            checkDefinition.Values = new[] {"6"};
            check = sut.CreateCheck(checkDefinition);
            actual = (StringLengthCheck)check;
            actual.MinimumLength.Should().Be(6);
            actual.MaximumLength.Should().Be(int.MaxValue);

            checkDefinition.Values = null;
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>()
                .WithMessage("At least a minimum string length must be specified");

            checkDefinition.Values = new []{"6", "4"};
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>();

            checkDefinition.Values = new[] { "1", "2", "3" };
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>()
                .WithMessage("At most minimum and maximum string length can be specified");

            checkDefinition.Values = new[] { "a", "4" };
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<FormatException>();

            checkDefinition.Values = new[] { "4", "b" };
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<FormatException>();
        }

        [Test]
        public void Support_Regex_Check()
        {
            var sut = new CheckFactory();

            const string hint = "Not a \"pattern\"";
            const string pattern = "pattern";
            var checkDefinition = new CheckDefinition {Type = "regex", Values = new[] {hint, pattern}};

            var check = sut.CreateCheck(checkDefinition);
            var actual = (StringRegExCheck) check;
            actual.Should().NotBeNull();
            actual.Validate(pattern);
            actual.ShouldFailWith<ArgumentException>("foobar", ex => ex.Message == hint);

            checkDefinition.Values = null;
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>()
                .WithMessage("At least a hint and one regex pattern must be specified");

            checkDefinition.Values = new string[]{};
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>()
                .WithMessage("At least a hint and one regex pattern must be specified");

            checkDefinition.Values = new[] {hint};
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>()
                .WithMessage("At least a hint and one regex pattern must be specified");
        }

        [Test]
        public void Support_Range_Check()
        {
            var sut = new CheckFactory();

            var checkDefinition = new CheckDefinition { Type = "range", Values = new[] { "0", "4.5" } };

            var check = sut.CreateCheck(checkDefinition);
            var actual = (RangeCheck)check;
            actual.Minimum.Should().Be(0);
            actual.Maximum.Should().Be(4.5);

            checkDefinition.Values = new[] {"5.1", "5.0"};
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentOutOfRangeException>();

            checkDefinition.Values = new[] { "a", "5" };
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<FormatException>();

            checkDefinition.Values = new[] { "5", "a" };
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<FormatException>();

            checkDefinition.Values = new[] { "5" };
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>()
                .WithMessage("Must sepcify minimum and maximum");

            checkDefinition.Values = new string[] { };
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>()
                .WithMessage("Must sepcify minimum and maximum");

            checkDefinition.Values = null;
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>()
                .WithMessage("Must sepcify minimum and maximum");

            checkDefinition.Values = new[] { "1", "2", "3" };
            sut.Invoking(x => x.CreateCheck(checkDefinition))
                .ShouldThrow<ArgumentException>()
                .WithMessage("Must sepcify minimum and maximum");
        }
    }
}