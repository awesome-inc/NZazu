using System;
using System.Threading;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Fields.Libs
{
    [TestFixtureFor(typeof(AutoCompleteManager))]
    // ReSharper disable InconsistentNaming
    internal class AutoCompleteManager_Should
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void a()
        {
            0.Invoking(x => new AutoCompleteManager(null)).Should().Throw<ArgumentNullException>();
        }
    }
}