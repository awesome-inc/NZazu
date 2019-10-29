using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuFieldFactory))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuFieldFactory_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuFieldFactory();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfFieldFactory>();
        }

        [Test]
        [TestCase(null, typeof(Label))]
        [TestCase("label", typeof(Label))]
        [TestCase("string", typeof(TextBox))]
        [TestCase("multiline", typeof(TextBox))]
        [TestCase("bool", typeof(CheckBox))]
        [TestCase("int", typeof(TextBox))]
        [TestCase("date", typeof(DatePicker))]
        [TestCase("double", typeof(TextBox))]
        [TestCase("option", typeof(ComboBox))]
        [TestCase("error", typeof(ErrorPanel))]
        [STAThread]
        public void Support(string fieldType, Type controlType)
        {
            var sut = new NZazuFieldFactory();
            sut.Use(Substitute.For<INZazuWpfView>());

            var field = sut.CreateField(new FieldDefinition {Key = "test", Type = fieldType, Description = "test"});

            field.Should().NotBeNull();

            var control = field.ValueControl;
            control.Should().BeOfType(controlType);
        }

        [Test]
        [STAThread]
        public void Have_Location_Field()
        {
            var sut = new NZazuFieldFactory();

            var fld = sut.CreateField(new FieldDefinition {Key = "test", Type = "location"});
            fld.ValueControl.Should().BeOfType<GeoLocationBox>();

            ((GeoLocationBox) fld.ValueControl).GeoLocationSupport.Should().NotBeNull();
        }

        [Test]
        [TestCase("group", null, typeof(ContentControl))]
        [TestCase("group", "header", typeof(GroupBox))]
        [STAThread]
        public void Support_GroupBox(string fieldType, string description, Type controlType)
        {
            var sut = new NZazuFieldFactory();

            var field = sut.CreateField(new FieldDefinition
                {Key = "test", Type = fieldType, Description = description});

            field.Should().NotBeNull();

            var control = field.ValueControl;
            control.Should().BeOfType(controlType);
        }

        [Test]
        [STAThread]
        public void Return_Label_If_Not_Supported()
        {
            var sut = new NZazuFieldFactory();

            var field =
                sut.CreateField(new FieldDefinition
                {
                    Key = "test",
                    Type = "I am a not supported label",
                    Description = "test"
                });

            field.Should().NotBeNull();
            // because of the fallback in case of null

            var control = field.ValueControl;
            control.Should().BeOfType<Label>();
        }

        [Test]
        public void Use_CheckFactory_for_creating_checks()
        {
            var behaviorFactory = Substitute.For<INZazuWpfFieldBehaviorFactory>();
            var serializer = Substitute.For<INZazuTableDataSerializer>();
            var checkFactory = Substitute.For<ICheckFactory>();

            var checkDefinition = new CheckDefinition {Type = "required"};
            var fieldDefinition = new FieldDefinition {Key = "test", Type = "string", Checks = new[] {checkDefinition}};
            var check = new RequiredCheck(checkDefinition.Settings, null, null, 0, fieldDefinition);
            checkFactory.CreateCheck(checkDefinition, fieldDefinition, Arg.Any<Func<FormData>>(),
                Arg.Any<INZazuTableDataSerializer>(), Arg.Any<int>()).Returns(check);

            var sut = new NZazuFieldFactory();
            sut.Use(behaviorFactory);
            sut.Use(checkFactory);
            sut.Use(serializer);

            var field = (NZazuField) sut.CreateField(fieldDefinition);

            checkFactory.Received(1).CreateCheck(checkDefinition, fieldDefinition, Arg.Any<Func<FormData>>(),
                Arg.Any<INZazuTableDataSerializer>(), Arg.Any<int>());

            field.Should().NotBeNull();
            field.Check.Should().Be(check);
        }

        [Test]
        public void Use_AggregateCheck_for_multiple_checks()
        {
            var behaviorFactory = Substitute.For<INZazuWpfFieldBehaviorFactory>();
            var serializer = Substitute.For<INZazuTableDataSerializer>();
            var checkFactory = Substitute.For<ICheckFactory>();

            var checkDefinition1 = new CheckDefinition {Type = "required"};
            var checkDefinition2 = new CheckDefinition
                {Type = "length", Settings = new Dictionary<string, string> {{"Min", "4"}, {"Max", "6"}}};
            var fieldDefinition = new FieldDefinition
            {
                Key = "test",
                Type = "string",
                Checks = new[] {checkDefinition1, checkDefinition2}
            };
            var check1 = new RequiredCheck(checkDefinition1.Settings, null, null, 0, fieldDefinition);
            var check2 = new StringLengthCheck(checkDefinition2.Settings, null, null, 0, fieldDefinition);

            checkFactory.CreateCheck(checkDefinition1, fieldDefinition, Arg.Any<Func<FormData>>(),
                Arg.Any<INZazuTableDataSerializer>(), Arg.Any<int>()).Returns(check1);
            checkFactory.CreateCheck(checkDefinition2, fieldDefinition, Arg.Any<Func<FormData>>(),
                Arg.Any<INZazuTableDataSerializer>(), Arg.Any<int>()).Returns(check2);

            var sut = new NZazuFieldFactory();
            sut.Use(behaviorFactory);
            sut.Use(checkFactory);
            sut.Use(serializer);

            var field = (NZazuField) sut.CreateField(fieldDefinition);

            checkFactory.Received(1).CreateCheck(checkDefinition1, fieldDefinition, Arg.Any<Func<FormData>>(),
                Arg.Any<INZazuTableDataSerializer>(), Arg.Any<int>());
            checkFactory.Received(1).CreateCheck(checkDefinition2, fieldDefinition, Arg.Any<Func<FormData>>(),
                Arg.Any<INZazuTableDataSerializer>(), Arg.Any<int>());

            var aggregateCheck = (AggregateCheck) field.Check;
            aggregateCheck.Should().NotBeNull();
            aggregateCheck.Checks.Should().BeEquivalentTo(check1, check2);
        }

        [Test]
        public void Copy_Settings_from_FieldDefinition()
        {
            var sut = new NZazuFieldFactory();
            var fieldDefinition = new FieldDefinition {Key = "test", Type = "string"};
            var field = (NZazuField) sut.CreateField(fieldDefinition);
            field.Definition.Settings.Should().NotBeNull();
            field.Definition.Settings.Should().BeEmpty();

            var settings = new Dictionary<string, string> {{"key", "value"}};
            fieldDefinition.Settings = settings;

            field = (NZazuField) sut.CreateField(fieldDefinition);
            field.Definition.Settings.Should().BeEquivalentTo(settings);
        }

        [Test]
        public void Does_Attach_Behavior_To_Field()
        {
            // because view has the resolver and attaches the behavior
            var sut = new NZazuFieldFactory();
#pragma warning disable 618
            var field = (NZazuField) sut.CreateField(
                new FieldDefinition
                {
                    Key = "test",
                    Type = "string",
                    Behaviors = new[] {new BehaviorDefinition {Name = "Empty"}}
                });
            field.Should().NotBeNull();
#pragma warning restore 618
            field.Behaviors.Count().Should().Be(1);
        }

        [Test]
        public void Does_Attach_Behaviors_To_Field()
        {
            // because view has the resolver and attaches the behavior
            var sut = new NZazuFieldFactory();
            var fieldDef = new FieldDefinition
            {
                Key = "test",
                Type = "string",
                Behaviors = new List<BehaviorDefinition>
                {
                    new BehaviorDefinition {Name = "behaviorDefinition1"},
                    new BehaviorDefinition {Name = "behaviorDefinition2"},
                    new BehaviorDefinition {Name = "behaviorDefinition3"}
                }
            };

            var field = (NZazuField) sut.CreateField(fieldDef);
            field.Should().NotBeNull();
            field.Behaviors.Should().NotBeNull();
            field.Behaviors.Count().Should().Be(3);
        }

        [Test]
        public void Does_Attach_Behavior_And_Behaviors_To_Field()
        {
            // because view has the resolver and attaches the behavior
            var sut = new NZazuFieldFactory();

            var fieldDef = new FieldDefinition
            {
                Key = "test",
                Type = "string",
                Behaviors = new List<BehaviorDefinition>
                {
                    new BehaviorDefinition {Name = "behaviorDefinition1"},
                    new BehaviorDefinition {Name = "behaviorDefinition2"},
                    new BehaviorDefinition {Name = "behaviorDefinition3"}
                }
            };

            var field = (NZazuField) sut.CreateField(fieldDef);
            field.Should().NotBeNull();
            field.Behaviors.Should().NotBeNull();
            field.Behaviors.Count().Should().Be(3);
        }

        [Test]
        public void Recursively_create_GroupFields()
        {
            var sut = new NZazuFieldFactory();

            var fields = new[]
            {
                new FieldDefinition
                {
                    Key = "first",
                    Type = "string"
                },
                new FieldDefinition
                {
                    Key = "second",
                    Type = "string"
                }
            };

            var fieldDefinition = new FieldDefinition
            {
                Key = "group1",
                Type = "group",
                Fields = fields
            };
            var field = (INZazuWpfFieldContainer) sut.CreateField(fieldDefinition);

            field.Should().NotBeNull();

            field.Fields.Should().HaveCount(fieldDefinition.Fields.Count());
        }

        [Test]
        public void Copy_group_layout()
        {
            var sut = new NZazuFieldFactory();

            var fieldDefinition = new FieldDefinition
            {
                Key = "group1",
                Type = "group",
                Layout = "grid"
            };
            var field = (INZazuWpfFieldContainer) sut.CreateField(fieldDefinition);

            field.Layout.Should().Be(fieldDefinition.Layout);
        }

        [Test]
        public void Copy_values_for_option_field()
        {
            var sut = new NZazuFieldFactory();

            var fieldDefinition = new FieldDefinition
            {
                Key = "test",
                Type = "option",
                Values = new[] {"1", "2", "3"}
            };

            var field = (NZazuOptionsField) sut.CreateField(fieldDefinition);

            field.Options.Should().BeEquivalentTo(fieldDefinition.Values);
        }

        [Test]
        public void Copy_values_for_keyedoption_field()
        {
            var sut = new NZazuFieldFactory();

            var fieldDefinition = new FieldDefinition
            {
                Key = "test",
                Type = "keyedoption",
                Values = new[] {"1", "2", "3"}
            };

            var field = (NZazuKeyedOptionsField) sut.CreateField(fieldDefinition);

            field.Options.Should().BeEquivalentTo(fieldDefinition.Values);
        }

        [Test]
        [STAThread]
        public void Attach_And_Detach_Behavior_To_Fields()
        {
            // lets mock the behavior and all the other stuff
            var behavior = Substitute.For<INZazuWpfFieldBehavior>();
            var behaviorFactory = Substitute.For<INZazuWpfFieldBehaviorFactory>();
            var checkFactory = Substitute.For<ICheckFactory>();
            var serializer = Substitute.For<INZazuTableDataSerializer>();

            var behaviorDefinition = new BehaviorDefinition {Name = "Empty"};
            var fieldDefinition = new FieldDefinition
                {Key = "a", Type = "string", Behaviors = new[] {behaviorDefinition}};
            behaviorFactory.CreateFieldBehavior(behaviorDefinition).Returns(behavior);


            // make sure an attach happens
            var sut = new NZazuFieldFactory();
            sut.Use(behaviorFactory);
            sut.Use(checkFactory);
            sut.Use(serializer);

            var fld1 = sut.CreateField(fieldDefinition);

            behaviorFactory.Received(1).CreateFieldBehavior(behaviorDefinition);
            behavior.Received(1).AttachTo(Arg.Any<INZazuWpfField>(), Arg.Any<INZazuWpfView>());
            behavior.ClearReceivedCalls();

            fld1.Dispose();

            // now lets create a ner form and detach the existing behavior
            behavior.ReceivedWithAnyArgs(1).Detach();
        }

        [Test]
        [STAThread]
        public void Attach_Multiple_Behaviors_To_Field()
        {
            // lets mock the behaviors and all the other stuff
            var behavior1 = Substitute.For<INZazuWpfFieldBehavior>();
            var behavior2 = Substitute.For<INZazuWpfFieldBehavior>();
            var behavior3 = Substitute.For<INZazuWpfFieldBehavior>();

            var behaviorFactory = Substitute.For<INZazuWpfFieldBehaviorFactory>();
            var checkFactory = Substitute.For<ICheckFactory>();
            var serializer = Substitute.For<INZazuTableDataSerializer>();

            // Append value to "StringValue" prop on each behaviour to simulate execution of behaviors
            behavior1.When(x => x.AttachTo(Arg.Any<INZazuWpfField>(), Arg.Any<INZazuWpfView>()))
                .Do(x => ((INZazuWpfField) x.Args()[0]).SetValue(
                    ((INZazuWpfField) x.Args()[0]).GetValue() + "behavior 1 executed!"));
            behavior2.When(x => x.AttachTo(Arg.Any<INZazuWpfField>(), Arg.Any<INZazuWpfView>()))
                .Do(x => ((INZazuWpfField) x.Args()[0]).SetValue(
                    ((INZazuWpfField) x.Args()[0]).GetValue() + "|behavior 2 executed!"));
            behavior3.When(x => x.AttachTo(Arg.Any<INZazuWpfField>(), Arg.Any<INZazuWpfView>()))
                .Do(x => ((INZazuWpfField) x.Args()[0]).SetValue(
                    ((INZazuWpfField) x.Args()[0]).GetValue() + "|behavior 3 executed!"));

            var fieldDefinition = new FieldDefinition
            {
                Key = "a",
                Type = "string",
                Behaviors = new List<BehaviorDefinition>
                {
                    new BehaviorDefinition {Name = "behaviorDefinition1"},
                    new BehaviorDefinition {Name = "behaviorDefinition2"},
                    new BehaviorDefinition {Name = "behaviorDefinition3"}
                }
            };

            behaviorFactory
                .CreateFieldBehavior(fieldDefinition.Behaviors.FirstOrDefault(b => b.Name == "behaviorDefinition1"))
                .Returns(behavior1);
            behaviorFactory
                .CreateFieldBehavior(fieldDefinition.Behaviors.FirstOrDefault(b => b.Name == "behaviorDefinition2"))
                .Returns(behavior2);
            behaviorFactory
                .CreateFieldBehavior(fieldDefinition.Behaviors.FirstOrDefault(b => b.Name == "behaviorDefinition3"))
                .Returns(behavior3);

            // make sure an attach happens
            var sut = new NZazuFieldFactory();
            sut.Use(behaviorFactory);
            sut.Use(checkFactory);
            sut.Use(serializer);
            var fld1 = sut.CreateField(fieldDefinition);

            // Check if behaviors were executed
            fld1.GetValue().Should().Be("behavior 1 executed!|behavior 2 executed!|behavior 3 executed!");

            //behavior1.When(x => x.AttachTo(fld1, Arg.Any<INZazuWpfView>())).Do(x => fld1.StringValue = "behaviorDefinition1");
            behavior1.Received(1).AttachTo(Arg.Any<INZazuWpfField>(), Arg.Any<INZazuWpfView>());
            behavior1.ClearReceivedCalls();

            //behavior2.When(x => x.AttachTo(fld1, Arg.Any<INZazuWpfView>())).Do(x => fld1.StringValue = "behaviorDefinition2");
            behavior2.Received(1).AttachTo(Arg.Any<INZazuWpfField>(), Arg.Any<INZazuWpfView>());
            behavior2.ClearReceivedCalls();

            //behavior3.When(x => x.AttachTo(fld1, Arg.Any<INZazuWpfView>())).Do(x => fld1.StringValue = "behaviorDefinition3");
            behavior3.Received(1).AttachTo(Arg.Any<INZazuWpfField>(), Arg.Any<INZazuWpfView>());
            behavior3.ClearReceivedCalls();

            fld1.Dispose();

            // now lets create a ner form and detach the existing behavior
            behavior1.ReceivedWithAnyArgs(1).Detach();
            behavior2.ReceivedWithAnyArgs(1).Detach();
            behavior3.ReceivedWithAnyArgs(1).Detach();
        }
    }
}