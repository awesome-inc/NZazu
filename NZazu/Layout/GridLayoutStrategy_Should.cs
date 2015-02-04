using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using NZazu.Fields;

namespace NZazu.Layout
{
    [TestFixture, RequiresSTA]
    // ReSharper disable InconsistentNaming
    class GridLayoutStrategy_Should
    {
        private Application application;

        [SetUp]
        public void CreateApplicationForResources()
        {
            if (Application.Current != null) return;

            application = new Application();
        }

        [TearDown]
        public void RemoveApplicationForResources()
        {
            if (application == null) return;

            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            application.Dispatcher.InvokeShutdown();
            application = null;
        }

        [Test]
        public void Layout_fields_in_a_two_column_grid()
        {
            var sut = new GridLayoutStrategy();

            var container = new ContentControl();
            var fields = new[]
            {
                new NZazuField("label1") { Prompt="label prompt", Description = "label text"}, 
                new NZazuTextField("string1") { Prompt="text prompt", Description = "text tooltip"}, 
                new NZazuBoolField("bool1") { Prompt="bool prompt", Description = "checkbox tooltip"}
            };
            sut.DoLayout(container, fields);

            var grid = (Grid)container.Content;
            grid.Should().NotBeNull();

            var colDefs = grid.ColumnDefinitions;
            colDefs.Should().HaveCount(2);
            colDefs[0].Width.IsAuto.Should().BeTrue();
            colDefs[1].Width.IsStar.Should().BeTrue();

            var rowDefs = grid.RowDefinitions;
            rowDefs.Should().HaveCount(3);

            grid.Children.Should().HaveCount(2 * fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                grid.Children[2 * i].Should().Be(fields[i].LabelControl);
                grid.Children[2 * i + 1].Should().Be(fields[i].ValueControl);
            }
        }

        [Test]
        public void Skip_rows_if_label_and_value_both_empty()
        {
            var sut = new GridLayoutStrategy();

            var container = new ContentControl();
            var fields = new[]
            {
                new NZazuField("label1"),
                new NZazuTextField("string1"),
                new NZazuBoolField("bool1")
            };

            sut.DoLayout(container, fields);

            var grid = (Grid)container.Content;
            grid.Should().NotBeNull();

            grid.Children.Should().HaveCount(2);
            grid.Children[0].Should().Be(fields[1].ValueControl, "label should be skipped");
            grid.Children[1].Should().Be(fields[2].ValueControl, "label should be skipped");
        }

        [Test]
        public void Set_Validation_Error_Template()
        {
            var expectedTemplate = new ControlTemplate();
            var sut = new GridLayoutStrategy(expectedTemplate);

            var container = new ContentControl();
            var fields = new[]
            {
                new NZazuField("label1"),
                new NZazuTextField("string1"),
                new NZazuBoolField("bool1")
            };

            fields
                .Where(f => f.ValueControl != null)
                .All(f => Validation.GetErrorTemplate(f.ValueControl) != expectedTemplate)
                .Should().BeTrue();


            sut.DoLayout(container, fields);

            fields
                .Where(f => f.ValueControl != null)
                .All(f => Validation.GetErrorTemplate(f.ValueControl) == expectedTemplate)
                .Should().BeTrue();
        }
    }
}