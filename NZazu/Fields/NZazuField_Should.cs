using System;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class NZazuField_Should
    {
        #region private classes

        class TestField : NZazuField
        {
            public TestField(string key) : base(key) { }
        }

        #endregion

        [Test]
        public void Validate_ctor_parameters()
        {
            new Action(() => new TestField("")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new TestField(null)).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new TestField("\t\r\n ")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
        }
    }
}