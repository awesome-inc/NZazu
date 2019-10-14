using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.FieldBehavior
{
    [TestFixtureFor(typeof(BehaviorExtender))]
    // ReSharper disable once InconsistentNaming
    internal class BehaviorExtender_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new BehaviorExtender();
            sut.Should().NotBeNull();
        }

        [Test]
        public void Have_Behavior_As_Immutable_Enumeration()
        {
            var sut = new BehaviorExtender();

            sut.Behaviors.Should().BeAssignableTo<IEnumerable<KeyValuePair<string, Type>>>();
            sut.Should().NotBeNull();
        }

        [Test]
        public void Have_Empty_Registration_For_Testing()
        {
            var sut = new BehaviorExtender();

            sut.Behaviors.Should().Contain(kvp => string.Compare(kvp.Key, "Empty", StringComparison.Ordinal) == 0);
        }

        [Test]
        public void Add_And_Remove_Registration_Of_Additional_Behaviors()
        {
            const string name = "Mock";
            var type = typeof(DummyFieldBehavior);
            var sut = new BehaviorExtender();

            sut.RegisterType(name, type);
            sut.Behaviors.Should().Contain(kvp =>
                string.Compare(kvp.Key, name, StringComparison.Ordinal) == 0 && kvp.Value == type);

            sut.UnregisterType(name);
            sut.Behaviors.Should().NotContain(kvp => string.Compare(kvp.Key, name, StringComparison.Ordinal) == 0);
        }

        [Test]
        public void Do_Nothing_On_Remove_Registration_Of_Wrong_Name()
        {
            var name = "I do not exist as registration. " + Guid.NewGuid();
            var sut = new BehaviorExtender();

            sut.UnregisterType(name);
        }

        [Test]
        public void Registration_And_Unregistration_Through_Public_Method()
        {
            const string name = "Mock For a Static Thing Which Stays Forever In List";
            var type = typeof(DummyFieldBehavior);

            // thread-safe?
            lock (new object())
            {
                BehaviorExtender.Register(name, type);
                try
                {
                    var behaviors = BehaviorExtender.GetBehaviors().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    behaviors[name].Should().Be(type);
                    behaviors["Empty"].Should().Be(typeof(EmptyNZazuFieldBehavior));
                }
                finally
                {
                    BehaviorExtender.Unregister(name);
                }
            }
        }

        [Test]
        public void Support_type_safe_generic_registration()
        {
            lock (new object())
            {
                const string name = "dummy";
                BehaviorExtender.IsRegistered(name).Should().BeFalse();
                BehaviorExtender.Register<DummyFieldBehavior>(name);
                try
                {
                    BehaviorExtender.IsRegistered(name).Should().BeTrue();
                }
                finally
                {
                    BehaviorExtender.Unregister(name);
                }
            }
        }

        [Test]
        public void Replace_Existing_Registration()
        {
            const string name = "Empty";
            var type = typeof(DummyFieldBehavior);
            var sut = new BehaviorExtender();

            sut.RegisterType(name, type);

            sut.Behaviors.Should()
                .Contain(kvp => string.Compare(kvp.Key, name, StringComparison.Ordinal) == 0 && kvp.Value == type);
        }

        #region inner classes for testing

        private class DummyFieldBehavior : EmptyNZazuFieldBehavior
        {
        }

        #endregion
    }
}