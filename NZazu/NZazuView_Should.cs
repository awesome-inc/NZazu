using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;

namespace NZazu
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuView_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuView();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfView>();
        }

        [Test]
        public void Update_when_FormDefinition_changed()
        {
            var sut = new NZazuView();

            var layout = Substitute.For<INZazuWpfLayoutStrategy>();
            sut.LayoutStrategy = layout;
            layout.ClearReceivedCalls();

            sut.FormDefinition = new FormDefinition { Fields = new FieldDefinition[] { } };

            layout.Received().DoLayout(sut.Layout, Arg.Any<IEnumerable<INZazuWpfField>>());
        }

        [Test]
        public void Update_when_FieldFactory_changed()
        {
            var layout = Substitute.For<INZazuWpfLayoutStrategy>();
            var sut = new NZazuView();
            sut.LayoutStrategy = layout;
            sut.FieldFactory = Substitute.For<INZazuWpfFieldFactory>();
            sut.FormDefinition = new FormDefinition { Fields = new FieldDefinition[] { } };


            layout.ClearReceivedCalls();
            var fieldFactory = Substitute.For<INZazuWpfFieldFactory>();
            sut.FieldFactory = fieldFactory;

            layout.Received().DoLayout(sut.Layout, Arg.Any<IEnumerable<INZazuWpfField>>());
        }


        [Test]
        public void Update_when_LayoutStrategy_changed()
        {
            var sut = new NZazuView();
            sut.FieldFactory = Substitute.For<INZazuWpfFieldFactory>();
            sut.FormDefinition = new FormDefinition { Fields = new FieldDefinition[] { } };

            var layoutStrategy = Substitute.For<INZazuWpfLayoutStrategy>();

            // change strategy
            sut.LayoutStrategy = layoutStrategy;

            layoutStrategy.Received().DoLayout(sut.Layout, Arg.Any<IEnumerable<INZazuWpfField>>());
        }

        [Test]
        public void Disallow_null_FieldFactory()
        {
            var sut = new NZazuView();
            sut.FieldFactory.Should().NotBeNull();

            sut.FieldFactory = null;

            sut.FieldFactory.Should().NotBeNull();
        }

        [Test]
        public void Disallow_null_LayoutStrategy()
        {
            var sut = new NZazuView();
            sut.LayoutStrategy.Should().NotBeNull();

            sut.LayoutStrategy = null;

            sut.LayoutStrategy.Should().NotBeNull();
        }

        [Test]
        public void Set_Field_Values_If_FormData_Changes()
        {
            var view = new NZazuView();

            const string key = "name";
            const string value = "John";

            var formDefinition = new FormDefinition { Fields = new[] { new FieldDefinition { Key = key, Type = "string" } } };
            view.FormDefinition = formDefinition;

            view.FormData = new Dictionary<string, string> { { key, value } };

            var actual = view.GetFieldValues();
            actual.Keys.Single().Should().Be(key);
            actual[key].Should().Be(value);
        }

        [Test]
        public void Update_FormData_On_ApplyChanges()
        {
            var view = new NZazuView();

            const string key = "name";
            const string value = "John";

            var formDefinition = new FormDefinition { Fields = new[] { new FieldDefinition { Key = key, Type = "string" } } };
            view.FormDefinition = formDefinition;

            var input = new Dictionary<string, string> { { key, value } };
            view.SetFieldValues(input);

            view.FormData.Values.Should().BeEmpty();
            view.ApplyChanges();
            view.FormData.Values.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void Validate_By_Calling_INZazuField_Validate()
        {
            var field = Substitute.For<INZazuWpfField>();
            field.Key.ReturnsForAnyArgs("test");
            var fieldFactory = Substitute.For<INZazuWpfFieldFactory>();
            var layoutStrategy = Substitute.For<INZazuWpfLayoutStrategy>();
            fieldFactory.CreateField(Arg.Any<FieldDefinition>()).Returns(field);

            var sut = new NZazuView
            {
                FormDefinition = new FormDefinition {Fields = new [] {new FieldDefinition {Key = "test"}}},
                FieldFactory = fieldFactory,
                LayoutStrategy = layoutStrategy
            };

            sut.Validate();
            field.ReceivedWithAnyArgs().Validate();
        }

    }
}
