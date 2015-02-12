using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.FieldBehavior
{
    public class BehaviorExtender
    {
        #region lazy static singleton

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<BehaviorExtender> instance =
            new Lazy<BehaviorExtender>(() => new BehaviorExtender());

        public static BehaviorExtender Instance
        {
            get { return instance.Value; }
        }

        // wrapper for comfort. instance method set to private
        public static void Register(string name, Type type)
        {
            Instance.RegisterType(name, type);
        }

        // wrapper for comfort. instance method set to private
        public static void Unregister(string name)
        {
            Instance.UnregisterType(name);
        }

        public static IEnumerable<KeyValuePair<string, Type>> GetBehaviors()
        {
            return Instance.Behaviors;
        } 

        #endregion

        private readonly Dictionary<string, Type> _fieldTypes = new Dictionary<string, Type>();

        private BehaviorExtender()
        {
            // over here you can add new default behaviors
            _fieldTypes.Add("Empty", typeof(EmptyNZazuFieldBehavior));
        }

        internal IEnumerable<KeyValuePair<string, Type>> Behaviors
        {
            get { return _fieldTypes.ToArray(); }
        }

        private void RegisterType(string name, Type type)
        {
            if (_fieldTypes.ContainsKey(name))
            {
                Trace.TraceInformation("A registration for '{0}' already exists for {1}. Replacing with {2}", name, _fieldTypes[name].Name, type.Name);
                _fieldTypes[name] = type;
            }
            else
                _fieldTypes.Add(name, type);
        }

        private void UnregisterType(string name)
        {
            if (_fieldTypes.ContainsKey(name))
                _fieldTypes.Remove(name);
            else
                Trace.TraceInformation("A registration for '{0}' does not exist. Nothing removed", name);
        }

        #region internal test due to private constructor

        [TestFixture]
        // ReSharper disable once InconsistentNaming
        private class BehaviorExtender_Should
        {
            #region inner classes for testing

            private class DummyFieldBehavior : EmptyNZazuFieldBehavior { }

            #endregion

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

                sut.Behaviors.Should().Contain(kvp => String.Compare(kvp.Key, "Empty", StringComparison.Ordinal) == 0);
            }

            [Test]
            public void Add_And_Remove_Registration_Of_Additional_Behaviors()
            {
                const string name = "Mock";
                var type = typeof(DummyFieldBehavior);
                var sut = new BehaviorExtender();

                sut.RegisterType(name, type);
                sut.Behaviors.Should().Contain(kvp => String.Compare(kvp.Key, name, StringComparison.Ordinal) == 0 && kvp.Value == type);

                sut.UnregisterType(name);
                sut.Behaviors.Should().NotContain(kvp => String.Compare(kvp.Key, name, StringComparison.Ordinal) == 0);
            }

            [Test]
            public void Do_Nothing_On_Remove_Registration_Of_Wrong_Name()
            {
                var name = "I do not exist as registration. " + Guid.NewGuid().ToString();
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
                        behaviors["Empty"].Should().Be(typeof (EmptyNZazuFieldBehavior));
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
                    .Contain(kvp => String.Compare(kvp.Key, name, StringComparison.Ordinal) == 0 && kvp.Value == type);
            }
        }

        #endregion
    }
}