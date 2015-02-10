using System;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu.FieldBehavior
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuFieldBehaviorFactory_Should
    {
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

        [Test]
        [TestCase(null)]
        [TestCase("Empty", typeof(EmptyNZazuFieldBehavior))]
        public void Support(string fieldType, Type controlType)
        {
            var sut = new NZazuFieldBehaviorFactory();

            var field = sut.CreateFieldBehavior(new BehaviorDefinition { Name = fieldType });
            field.Should().NotBeNull();
            field.GetType().Should().Be(controlType);
        }
    }

}