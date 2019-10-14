using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts.Checks;

namespace NZazu.Contracts
{
    [TestFixtureFor(typeof(CheckFactory))]
    // ReSharper disable InconsistentNaming
    internal class CheckFactory_Should
    {
        [Test]
        public void Throw_on_unsupported_types()
        {
            var sut = new CheckFactory();
            sut.Invoking(x => x.CreateCheck(null, null)).Should().Throw<ArgumentNullException>();
            sut.Invoking(x => x.CreateCheck(new CheckDefinition(), null)).Should().Throw<ArgumentException>()
                .WithMessage("check type not specified");
            sut.Invoking(x => x.CreateCheck(new CheckDefinition {Type = "foobar"}, null)).Should()
                .Throw<NotSupportedException>().WithMessage("The specified check 'foobar' is not supported");
        }

        [Test]
        public void Be_Creatable()
        {
            var ctx = new ContextFor<CheckFactory>();
            var sut = ctx.BuildSut();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<ICheckFactory>();
        }

        [Test]
        public void Print_Supported_Types()
        {
            var ctx = new ContextFor<CheckFactory>();
            var sut = ctx.BuildSut();

            foreach (var type in sut.AvailableTypes)
                Console.WriteLine($"factory '{typeof(CheckFactory).Name}' supports type:\t{type}");

            Assert.Pass();
        }

        [Test]
        public void Not_Contain_AggregateCheck()
        {
            var ctx = new ContextFor<CheckFactory>();
            var sut = ctx.BuildSut();

            sut.AvailableTypes.Should().NotContain(typeof(AggregateCheck).Name);
        }

        [Test]
        [TestCaseSource(typeof(Create_All_Checks_Data), nameof(Create_All_Checks_Data.TestCases))]
        public void Create_All_Checks(string type)
        {
            var ctx = new ContextFor<CheckFactory>();
            var sut = ctx.BuildSut();

            var settings = new Dictionary<string, string>
            {
                {"Hint", "This is a hint for the error message"},
                {"RegEx", "true|false"}
            };

            var checkDefinition = new CheckDefinition {Type = type, Settings = settings};
            var definition = new FieldDefinition
                {Key = "test_string", Type = "string", Checks = new[] {checkDefinition}};
            var field = sut.CreateCheck(definition.Checks.Single(), definition, () => new FormData(),
                Substitute.For<INZazuTableDataSerializer>());

            field.Should().NotBeNull();
        }

        #region testdata source

        [ExcludeFromCodeCoverage]
        [Because("this is a data helper")]
        private class Create_All_Checks_Data
        {
            public static IEnumerable<string> TestCases
            {
                get
                {
                    var ctx = new ContextFor<CheckFactory>();
                    var sut = ctx.BuildSut();
                    return sut.AvailableTypes;
                }
            }
        }

        #endregion
    }
}