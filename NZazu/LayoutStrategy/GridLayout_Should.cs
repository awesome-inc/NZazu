using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Fields;

namespace NZazu.LayoutStrategy
{
    [TestFixtureFor(typeof (GridLayout))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class GridLayout_Should
    {
        private Application application;

        [SetUp]
        [ExcludeFromCodeCoverage]
        public void Create_Application_For_Resources()
        {
            if (Application.Current != null) return;
            application = new Application();
        }

        [TearDown]
        [ExcludeFromCodeCoverage]
        public void RemoveApplicationForResources()
        {
            if (application == null) return;

            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            application.Dispatcher.InvokeShutdown();
            application = null;
        }

        [Test]
        [STAThread]
        public void Layout_fields_in_a_two_column_grid()
        {
            var sut = new GridLayout();

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField("label1") { Prompt="label prompt", Description = "label text"}, 
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
            rowDefs.All(r => r.Height == GridLength.Auto).Should().BeTrue();

            grid.Children.Should().HaveCount(2 * fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                grid.Children[2 * i].Should().Be(fields[i].LabelControl);
                grid.Children[2 * i + 1].Should().Be(fields[i].ValueControl);
            }
        }

        [Test]
        [STAThread]
        public void Skip_rows_if_label_and_value_both_empty()
        {
            var sut = new GridLayout();

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField("label1"),
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
        [STAThread]
        public void Set_Validation_Error_Template()
        {
            var expectedTemplate = new ControlTemplate();
            var sut = new GridLayout(expectedTemplate);

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField("label1"),
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

        [Test]
        [STAThread]
        public void Recurse_on_group_fields()
        {
            var sut = new GridLayout();//null, resolveLayout);

            var container = new ContentControl();
            var fields1 = new NZazuField[]
            {
                new NZazuLabelField("label1") { Description = "label1"},
                new NZazuTextField("string1"),
                new NZazuBoolField("bool1")
            };
            var fields2 = new NZazuField[]
            {
                new NZazuLabelField("label2")  { Description = "label2"},
                new NZazuTextField("string2"),
                new NZazuBoolField("bool2")
            };
            var groups = new[]
            {
                new NZazuGroupField("group1") { Fields = fields1 },
                new NZazuGroupField("group2") { Fields = fields2 }
            };

            sut.DoLayout(container, groups);

            var grid = (Grid)container.Content;
            grid.Should().NotBeNull();

            grid.Children.Should().HaveCount(2);
            var content1 = (ContentControl)groups[0].ValueControl;
            var content2 = (ContentControl)groups[1].ValueControl;
            grid.Children[0].Should().Be(content1);
            grid.Children[1].Should().Be(content2);

            var grid1 = (Grid)content1.Content;
            var grid2 = (Grid)content2.Content;

            grid1.Children.Should().HaveCount(fields1.Length);
            grid2.Children.Should().HaveCount(fields2.Length);
            for (int i = 0; i < fields1.Length; i++)
                grid1.Children[i].Should().Be(fields1[i].ValueControl);
            for (int i = 0; i < fields2.Length; i++)
                grid2.Children[i].Should().Be(fields2[i].ValueControl);
        }

        [Test]
        [STAThread]
        public void Use_resolve_layout_on_groupfields()
        {
            var resolveLayout = Substitute.For<IResolveLayout>();
            var stackLayout = Substitute.For<INZazuWpfLayoutStrategy>();
            var gridLayout = Substitute.For<INZazuWpfLayoutStrategy>();
            resolveLayout.Resolve("stack").Returns(stackLayout);
            resolveLayout.Resolve("grid").Returns(gridLayout);

            var sut = new GridLayout();


            var container = new ContentControl();
            var fields1 = new NZazuField[]
            {
                new NZazuLabelField("label1") { Description = "label1"},
                new NZazuTextField("string1"),
                new NZazuBoolField("bool1")
            };
            var fields2 = new NZazuField[]
            {
                new NZazuLabelField("label2")  { Description = "label2"},
                new NZazuTextField("string2"),
                new NZazuBoolField("bool2")
            };
            var groups = new[]
            {
                new NZazuGroupField("stack") { Fields = fields1, Layout = "stack"},
                new NZazuGroupField("grid") { Fields = fields2, Layout = "grid"}
            };

            sut.DoLayout(container, groups, resolveLayout);
            
            stackLayout.Received().DoLayout(Arg.Any<ContentControl>(), fields1, resolveLayout);
            //gridLayout.Received().DoLayout(Arg.Any<ContentControl>(), fields2);
        }
    }
}