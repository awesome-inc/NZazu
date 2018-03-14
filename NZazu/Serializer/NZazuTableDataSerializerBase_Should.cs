using System.Collections.Generic;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Serializer
{
    [TestFixtureFor(typeof(NZazuTableDataSerializerBase))]
    // ReSharper disable once InconsistentNaming
    internal class NZazuTableDataSerializerBase_Should
    {
        private Dictionary<string, string> GetTableData()
        {
            return new Dictionary<string, string>
            {
                { "table01_field01__1", "hello" }, { "table01_field02__1", "world" },
                { "table01_field01__2", "foo" }, { "table01_field02__2", "bar" },
            };
        }

        [Test]
        public void Add_Row_To_Dictionary()
        {
            var newRow = new Dictionary<string, string>() {
                { "table01_field01", "jane" }, { "table01_field02", "doe" },
            };

            var data = GetTableData();
            data.Count.Should().Be(4);

            var sut = new NZazuTableDataSerializerBase();
            sut.AddTableRow(data, newRow);

            data.Count.Should().Be(6);
            data.Should().Contain("table01_field01__3", "jane");
            data.Should().Contain("table01_field02__3", "doe");
        }
    }
}