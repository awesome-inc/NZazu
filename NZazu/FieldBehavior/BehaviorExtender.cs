using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

        // wrapper for comfort
        public static void Register(string name, Type type)
        {
            Instance.RegisterType(name, type);
        }

        #endregion

        private readonly Dictionary<string, Type> _fieldTypes = new Dictionary<string, Type>();

        private BehaviorExtender()
        {
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
                Trace.TraceInformation(" A regsitration for the key '{0}' already exists for {1}. Replacing registration with {2}", name, _fieldTypes[name].Name, type.Name);
                _fieldTypes[name] = type;
            }
            else
                _fieldTypes.Add(name, type);
        }

        #region internal test due to private contructor

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
            public void Have_Behavior_As_Immutal_Enumeration()
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
            public void Add_Registration_Of_Additional_Behaviors()
            {
                const string name = "Mock";
                var type = typeof(DummyFieldBehavior);
                var sut = new BehaviorExtender();

                sut.RegisterType(name, type);

                sut.Behaviors.Should()
                    .Contain(kvp => String.Compare(kvp.Key, name, StringComparison.Ordinal) == 0 && kvp.Value == type);
            }

            [Test]
            public void Registration_Through_Public_Method()
            {
                const string name = "Mock For a Static Thing Which Stays Forever In List";
                var type = typeof(DummyFieldBehavior);

                BehaviorExtender.Register(name, type);
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