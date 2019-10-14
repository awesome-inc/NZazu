using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [TestFixtureFor(typeof(FieldDefinition))]
    // ReSharper disable once InconsistentNaming
    internal class FieldDefinition_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new FieldDefinition();

            sut.Should().NotBeNull();
        }

        [Test]
        public void Not_Have_Settings_For_Additional_Config()
        {
            var sut = new FieldDefinition();

            sut.Should().NotBeNull();
            sut.Settings.Should().BeEmpty();
        }

        [Test]
        public void Not_Have_Behavior()
        {
            var sut = new FieldDefinition();
            sut.Behaviors.Should().BeEmpty();
        }

        [Test]
        public void Define_Behavior()
        {
            var sut = new FieldDefinition();
            var behavior1 = Substitute.For<BehaviorDefinition>();

            sut.Behaviors = new[] {behavior1};
            sut.Behaviors.Should().NotBeNull();
        }

        [Test]
        public void Define_Multiple_Behaviors()
        {
            var sut = new FieldDefinition();
            var behavior1 = Substitute.For<BehaviorDefinition>();
            var behavior2 = Substitute.For<BehaviorDefinition>();
            var behavior3 = Substitute.For<BehaviorDefinition>();

            sut.Behaviors = new List<BehaviorDefinition> {behavior1, behavior2, behavior3};

            sut.Behaviors.Should().NotBeNullOrEmpty();
            sut.Behaviors.Count().Should().Be(3);
        }

        [Test]
        public void Define_Single_Behavior_And_Multiple_Behaviors()
        {
            var sut = new FieldDefinition();
            var behavior2 = Substitute.For<BehaviorDefinition>();
            var behavior3 = Substitute.For<BehaviorDefinition>();
            var behavior4 = Substitute.For<BehaviorDefinition>();

            sut.Behaviors = new List<BehaviorDefinition> {behavior2, behavior3, behavior4};
            sut.Behaviors.Count().Should().Be(3);
        }
    }
}