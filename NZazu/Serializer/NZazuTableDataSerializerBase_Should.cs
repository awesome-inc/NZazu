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
        private static Dictionary<string, string> GetTableData()
        {
            return new Dictionary<string, string>
            {
                {"table01_field01__1", "hello"}, {"table01_field02__1", "world"},
                {"table01_field01__2", "foo"}, {"table01_field02__2", "bar"}
            };
        }

        [Test]
        public void Add_Row_To_Dictionary()
        {
            var newRow = new Dictionary<string, string>
            {
                {"table01_field01", "jane"}, {"table01_field02", "doe"}
            };

            var data = GetTableData();
            data.Count.Should().Be(4);

            var sut = new NZazuTableDataSerializerBase();
            sut.AddTableRow(data, newRow);

            data.Count.Should().Be(6);
            data.Should().Contain("table01_field01__3", "jane");
            data.Should().Contain("table01_field02__3", "doe");
        }

        [Test]
        public void Add_Row_To_Empty_Dictionary()
        {
            var newRow = new Dictionary<string, string>
            {
                {"table01_field01", "jane"}, {"table01_field02", "doe"}
            };

            var data = new Dictionary<string, string>();
            data.Count.Should().Be(0);

            var sut = new NZazuTableDataSerializerBase();
            sut.AddTableRow(data, newRow);

            data.Count.Should().Be(2);
            data.Should().Contain("table01_field01__1", "jane");
            data.Should().Contain("table01_field02__1", "doe");
        }

        [Test]
        public void Not_Add_Empty_Row_To_Dictionary()
        {
            var newRow = new Dictionary<string, string>();

            var data = GetTableData();
            data.Count.Should().Be(4);

            var sut = new NZazuTableDataSerializerBase();
            sut.AddTableRow(data, newRow);

            data.Count.Should().Be(4);
        }

        [Test]
        public void Override_Empty_Row()
        {
            var newRow = new Dictionary<string, string>
            {
                {"table01_field01", "jane"},
                {"table01_field02", "doe"}
            };

            var data = new Dictionary<string, string> {{"table01_field01__1", ""}, {"table01_field02__1", null}};
            data.Count.Should().Be(2);

            var sut = new NZazuTableDataSerializerBase();
            sut.AddTableRow(data, newRow);

            data.Count.Should().Be(2);
            data.Should().Contain("table01_field01__1", "jane");
            data.Should().Contain("table01_field02__1", "doe");
        }
    }
}