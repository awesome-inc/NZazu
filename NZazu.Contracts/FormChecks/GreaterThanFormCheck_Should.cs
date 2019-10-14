using System.Collections.Generic;
using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.FormChecks
{
    [TestFixtureFor(typeof(GreaterThanFormCheck))]
    // ReSharper disable InconsistentNaming
    internal class GreaterThanFormCheck_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var settings = new Dictionary<string, string>
            {
                {"Hint", "this is the hint"},
                {"LeftFieldName", "leftField"},
                {"RightFieldName", "rightField"}
            } as IDictionary<string, string>;

            var ctx = new ContextFor<GreaterThanFormCheck>();
            ctx.Use(settings);
            var sut = ctx.BuildSut();

            sut.Should().NotBeNull();
        }

        [Test]
        public void Deal_With_Not_Exiting_Fields()
        {
            var settings = new Dictionary<string, string>
            {
                {"Hint", "this is the hint"},
                {"LeftFieldName", "leftField"},
                {"RightFieldName", "rightField"}
            } as IDictionary<string, string>;

            var sut = new GreaterThanFormCheck(settings);
            FormData foo = new Dictionary<string, string>
            {
                {"leftField", ""},
                {"rightField", ""}
            };

            sut.Validate(foo);
        }
    }
}