using System;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu.FieldBehavior
{
    [TestFixture, RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuFieldBehaviorFactory_Should
    {
        #region inner classes for testing

        private class DummyFieldBehavior : INZazuWpfFieldBehavior
        {
            public void AttachTo(Control valueControl)
            {
            }

            public void Detach()
            {
            }
        }

        #endregion

        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuFieldBehaviorFactory();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfFieldBehaviorFactory>();
        }

        [Test]
        public void Verify_Parameter_On_CreateFieldBehavior()
        {
            var sut = new NZazuFieldBehaviorFactory();

            new Action(() => sut.CreateFieldBehavior(null))
                .Invoking(a => a())
                .ShouldThrow<ArgumentNullException>();

            new Action(() => sut.CreateFieldBehavior(new BehaviorDefinition()))
                .Invoking(a => a())
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Return_Null_For_Unknown_Types()
        {
            var sut = new NZazuFieldBehaviorFactory();

            var behavior = sut.CreateFieldBehavior(new BehaviorDefinition { Name = "i am not registered" });
            behavior.Should().BeNull();
        }

        [Test, Ignore("TODO")]
        public void Allow_Registration_Of_Additional_Behaviors()
        {
            const string name = "mock";
            var type = typeof(DummyFieldBehavior);
            var sut = new NZazuFieldBehaviorFactory();

            sut.Register(name, type);

            var behavior = sut.CreateFieldBehavior(new BehaviorDefinition { Name = name });
            behavior.Should().NotBeNull();
        }

        [Test, Ignore("TODO")]
        [TestCase(null)]
        [TestCase("Empty", typeof(Label))]
        public void Support(string fieldType, Type controlType)
        {
            var sut = new NZazuFieldBehaviorFactory();

            //var field = sut.CreateFieldBehavior(new FieldDefinition { Key = "test", Type = fieldType, Description = "test" });

            //field.Should().NotBeNull();
            //field.Type.Should().Be(fieldType ?? "label"); // because of the fallback in case of null

            //var control = field.ValueControl;
            //control.Should().BeOfType(controlType);
            Assert.Inconclusive("implement me");
        }
    }

}