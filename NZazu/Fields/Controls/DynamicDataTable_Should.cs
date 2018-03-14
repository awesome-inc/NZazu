using System.Threading;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Fields.Controls
{
    [TestFixtureFor(typeof(DynamicDataTable))]
#pragma warning disable 618
    // ReSharper disable once InconsistentNaming
    internal class DynamicDataTable_Should
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Be_Creatable()
        {
            var sut = new DynamicDataTable();

            sut.Should().NotBeNull();
        }
    }
#pragma warning restore 618
}