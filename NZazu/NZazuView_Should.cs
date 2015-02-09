using System;
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
            var formDefinition = new FormDefinition { Layout = "grid", Fields = new FieldDefinition[] { } };
            VerifyUpdate(view =>
            {
                view.FormDefinition = formDefinition;
                view.FormDefinition.Should().Be(formDefinition);
            });
        }

        [Test]
        public void Update_when_FieldFactory_changed()
        {
            var fieldFactory = Substitute.For<INZazuWpfFieldFactory>();
            VerifyUpdate(view =>
            {
                view.FieldFactory = fieldFactory;
                view.FieldFactory.Should().Be(fieldFactory);
            });
        }

        [Test]
        public void Update_when_LayoutStrategy_changed()
        {
            var layout = Substitute.For<IResolveLayout>();
            VerifyUpdate(view =>
            {
                view.ResolveLayout = layout;
                view.ResolveLayout.Should().Be(layout);
            });
        }

        static void VerifyUpdate(Action<INZazuWpfView> act)
        {
            var formDefinition = new FormDefinition { Fields = new FieldDefinition[] { } };

            var resolveLayout = Substitute.For<IResolveLayout>();
            var layout = Substitute.For<INZazuWpfLayoutStrategy>();
            resolveLayout.Resolve(formDefinition.Layout).Returns(layout);
            var fieldFactory = Substitute.For<INZazuWpfFieldFactory>();

            var sut = new NZazuView
            {
                ResolveLayout = resolveLayout,
                FieldFactory = fieldFactory,
                FormDefinition = formDefinition
            };

            fieldFactory.ClearReceivedCalls();
            resolveLayout.ClearReceivedCalls();
            layout.ClearReceivedCalls();

            act(sut);

            sut.ResolveLayout.Received().Resolve(sut.FormDefinition.Layout);
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
            sut.ResolveLayout.Should().NotBeNull();

            sut.ResolveLayout = null;

            sut.ResolveLayout.Should().NotBeNull();
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
            var layout = Substitute.For<IResolveLayout>();
            fieldFactory.CreateField(Arg.Any<FieldDefinition>()).Returns(field);

            var sut = new NZazuView
            {
                FormDefinition = new FormDefinition {Fields = new [] {new FieldDefinition {Key = "test"}}},
                FieldFactory = fieldFactory,
                ResolveLayout = layout
            };

            sut.Validate();
            field.ReceivedWithAnyArgs().Validate();
        }

        [Test]
        [Description("In real-time scenarios try to preserve formdat when formdefinition changed only marginally")]
        public void Try_to_reapply_Formdata_if_FormDefinition_changed()
        {
            const string key = "name";
            const string value = "John";

            var fieldDefinition = new FieldDefinition { Key = key, Type = "string", Prompt = "Name" };
            var formDefinition = new FormDefinition { Fields = new[] { fieldDefinition } };
            var formData = new FormData(new Dictionary<string, string>{{key,value}});

            var sut = new NZazuView {FormDefinition = formDefinition, FormData = formData};

            sut.FormData.Should().Be(formData);
            var actual = sut.GetFieldValues();
            formData.Values.ShouldBeEquivalentTo(actual);

            fieldDefinition.Prompt = "Login";
            sut.FormDefinition = formDefinition;

            sut.FormData.Should().Be(formData);
            actual = sut.GetFieldValues();
            formData.Values.ShouldBeEquivalentTo(actual);
        }
    }
}
