using System;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class NZazuField_Should
    {
        [Test]
        public void Validate_ctor_parameters()
        {
            new Action(() => new TestField("", "", "")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new TestField(null, "", "")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new TestField("\t\r\n ", "", "")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
        }

        class TestField : NZazuField
        {
            public TestField(string key, string prompt, string description) : base(key, prompt, description)
            {
            }

            public override string Type
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}