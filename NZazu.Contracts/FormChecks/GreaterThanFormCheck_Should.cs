using System.Collections.Generic;
using NEdifis.Attributes;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NZazu.Contracts.FormChecks
{
    [TestFixtureFor(typeof(GreaterThanFormCheck))]
    // ReSharper disable InconsistentNaming
    public class GreaterThanFormCheck_Should
    {
        [Test]
        public void Deal_With_Not_Exiting_Fields()
        {
            var sut = new GreaterThanFormCheck("lorem ipsum", "left", "right");
            FormData foo = new Dictionary<string, string>();

        }
    }
}