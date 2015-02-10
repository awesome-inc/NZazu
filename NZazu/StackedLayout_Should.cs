using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using NZazu.Fields;

namespace NZazu
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class StackedLayout_Should
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
        public void Be_Creatable()
        {
            var sut = new StackedLayout();
            sut.Should().NotBeNull();

            sut.Should().BeAssignableTo<INZazuWpfLayoutStrategy>();
        }

        [Test(Description = "the stack panel should allign control groups from lest to right")]
        public void Align_Horizontally_And_Vertical_Top()
        {
            var sut = new StackedLayout();

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField("label1"),
            };

            sut.DoLayout(container, fields);

            var panel = (StackPanel)container.Content;
            panel.Should().NotBeNull();
            panel.Orientation.Should().Be(Orientation.Horizontal);
            panel.VerticalAlignment.Should().Be(VerticalAlignment.Top);
        }

        [Test]
        public void Skip_rows_if_label_and_value_both_empty()
        {
            var sut = new StackedLayout();

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField("label1"),
                new NZazuTextField("string1"),
                new NZazuBoolField("bool1")
            };

            sut.DoLayout(container, fields);

            var panel = (StackPanel)container.Content;
            panel.Should().NotBeNull();

            panel.Children.Should().HaveCount(2);
            panel.Children[0].Should().Be(fields[1].ValueControl, "label should be skipped");
            panel.Children[1].Should().Be(fields[2].ValueControl, "label should be skipped");
        }

        [Test]
        public void Add_controls()
        {
            var sut = new StackedLayout();

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField("label1") {Prompt = "heading"},
                new NZazuTextField("string1"),
                new NZazuBoolField("bool1")
            };

            sut.DoLayout(container, fields);

            var panel = (StackPanel)container.Content;
            panel.Should().NotBeNull();

            panel.Children.Should().HaveCount(3);
            panel.Children[0].Should().Be(fields[0].LabelControl, because: "we have a label bu no 'value'");
            panel.Children[1].Should().Be(fields[1].ValueControl, because: "we have no labels");
            panel.Children[2].Should().Be(fields[2].ValueControl, because: "we have no labels");
        }

        [Test]
        public void Set_Validation_Error_Template()
        {
            var expectedTemplate = new ControlTemplate();
            var sut = new StackedLayout(expectedTemplate);

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


    }
}