using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu.FieldBehavior
{
    [TestFixtureFor(typeof(NZazuFieldBehaviorFactory))]
    // ReSharper disable InconsistentNaming
    internal class NZazuFieldBehaviorFactory_Should
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
                .Should().Throw<ArgumentNullException>();

            new Action(() => sut.CreateFieldBehavior(new BehaviorDefinition()))
                .Invoking(a => a())
                .Should().Throw<ArgumentException>();
        }

        [Test]
        public void Handle_Interface_Implementations()
        {
            const string behaviorName = "IfaceImpl";
            BehaviorExtender.Register<SimpleInterfaceImplementation>(behaviorName);
            try
            {
                var sut = new NZazuFieldBehaviorFactory();
                var behavior = sut.CreateFieldBehavior(new BehaviorDefinition {Name = behaviorName});
                behavior.Should().BeAssignableTo<SimpleInterfaceImplementation>();

                // just to get code coverage
                behavior.AttachTo(null, null);
                behavior.Detach();
            }
            finally
            {
                BehaviorExtender.Unregister(behaviorName);
            }
        }

        [Test]
        public void Return_Null_For_Unknown_Types()
        {
            var sut = new NZazuFieldBehaviorFactory();

            var behavior = sut.CreateFieldBehavior(new BehaviorDefinition {Name = "i am not registered"});
            behavior.Should().BeNull();
        }

        [Test]
        [TestCase("Empty", typeof(EmptyNZazuFieldBehavior))]
        public void Support(string fieldType, Type controlType)
        {
            var sut = new NZazuFieldBehaviorFactory();

            var field = sut.CreateFieldBehavior(new BehaviorDefinition {Name = fieldType});
            field.Should().NotBeNull();
            field.GetType().Should().Be(controlType);
        }

        [Test]
        public void Respect_generic_settings()
        {
            const string behaviorName = "behaviorWithSettings";
            BehaviorExtender.Register<BehaviorWithSettings>(behaviorName);
            try
            {
                var sut = new NZazuFieldBehaviorFactory();
                var behaviorDefinition = new BehaviorDefinition
                {
                    Name = behaviorName,
                    Settings = new Dictionary<string, string>
                    {
                        {"AnInt", "42"},
                        {"AString", "AString"},
                        {"ADouble", "42.42"},
                        {"ABool", "True"}
                    }
                };
                var behavior = (BehaviorWithSettings) sut.CreateFieldBehavior(behaviorDefinition);

                behavior.AnInt.Should().Be(42);
                behavior.AString.Should().Be("AString");
                behavior.ADouble.Should().Be(42.42);
                behavior.ABool.Should().BeTrue();
            }
            finally
            {
                BehaviorExtender.Unregister(behaviorName);
            }
        }

        #region simple interface implementation

        [ExcludeFromCodeCoverage]
        // ReSharper disable once ClassNeverInstantiated.Local
        private class SimpleInterfaceImplementation : INZazuWpfFieldBehavior
        {
            public void AttachTo(INZazuWpfField field, INZazuWpfView view)
            {
            }

            public void Detach()
            {
            }
        }

        #endregion

        [ExcludeFromCodeCoverage]
        // ReSharper disable once ClassNeverInstantiated.Local
        private class BehaviorWithSettings : INZazuWpfFieldBehavior
        {
            public void AttachTo(INZazuWpfField field, INZazuWpfView view)
            {
            }

            public void Detach()
            {
            }

            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int AnInt { get; set; }
            public string AString { get; set; }
            public double ADouble { get; set; }

            public bool ABool { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }
}