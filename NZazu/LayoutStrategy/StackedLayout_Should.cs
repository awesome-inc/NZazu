using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;
using NZazu.Fields;

namespace NZazu.LayoutStrategy
{
    [TestFixtureFor(typeof(StackedLayout))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class StackedLayout_Should
    {
        private Application application;

        private object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

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
        public void Be_Creatable()
        {
            var sut = new StackedLayout();
            sut.Should().NotBeNull();

            sut.Should().BeAssignableTo<INZazuWpfLayoutStrategy>();
        }

        [Test(Description = "the stack panel should allign control groups from lest to right")]
        [STAThread]
        public void Align_Horizontally_And_Vertical_Top()
        {
            var sut = new StackedLayout();

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField(new FieldDefinition {Key = "label01"}, ServiceLocator)
            };

            sut.DoLayout(container, fields);

            var panel = (StackPanel) container.Content;
            panel.Should().NotBeNull();
            panel.Orientation.Should().Be(Orientation.Horizontal);
            panel.VerticalAlignment.Should().Be(VerticalAlignment.Top);
        }

        [Test]
        [STAThread]
        public void Skip_rows_if_label_and_value_both_empty()
        {
            var sut = new StackedLayout();

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField(new FieldDefinition {Key = "lable01"}, ServiceLocator),
                new NZazuTextField(new FieldDefinition {Key = "text01"}, ServiceLocator),
                new NZazuBoolField(new FieldDefinition {Key = "bool01"}, ServiceLocator)
            };

            sut.DoLayout(container, fields);

            var panel = (StackPanel) container.Content;
            panel.Should().NotBeNull();

            panel.Children.Should().HaveCount(2);
            panel.Children[0].Should().Be(fields[1].ValueControl, "label should be skipped");
            panel.Children[1].Should().Be(fields[2].ValueControl, "label should be skipped");
        }

        [Test]
        [STAThread]
        public void Add_controls()
        {
            var sut = new StackedLayout();

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField(new FieldDefinition {Key = "label01", Prompt = "heading"}, ServiceLocator),
                new NZazuTextField(new FieldDefinition {Key = "text01"}, ServiceLocator),
                new NZazuBoolField(new FieldDefinition {Key = "bool01"}, ServiceLocator)
            };

            sut.DoLayout(container, fields);

            var panel = (StackPanel) container.Content;
            panel.Should().NotBeNull();

            panel.Children.Should().HaveCount(3);
            panel.Children[0].Should().Be(fields[0].LabelControl, "we have a label bu no 'value'");
            panel.Children[1].Should().Be(fields[1].ValueControl, "we have no labels");
            panel.Children[2].Should().Be(fields[2].ValueControl, "we have no labels");
        }

        [Test]
        [STAThread]
        public void Set_Validation_Error_Template()
        {
            var expectedTemplate = new ControlTemplate();
            var sut = new StackedLayout(expectedTemplate);

            var container = new ContentControl();
            var fields = new NZazuField[]
            {
                new NZazuLabelField(new FieldDefinition {Key = "label01"}, ServiceLocator),
                new NZazuTextField(new FieldDefinition {Key = "text01"}, ServiceLocator),
                new NZazuBoolField(new FieldDefinition {Key = "bool01"}, ServiceLocator)
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