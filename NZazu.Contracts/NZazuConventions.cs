using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NEdifis.Attributes;
using NEdifis.Conventions;
using NUnit.Framework;

namespace NZazu.Contracts
{
    [ExcludeFromCodeCoverage]
    [Because("Coverage on tests?")]
    internal class NZazuConventions : ConventionBase
    {
        private static readonly IEnumerable<Type> TypesToTest = ClassesToTestFor<NZazuConventions>();

        public NZazuConventions()
        {
            // NEdifis built-in
            Conventions.AddRange(new IVerifyConvention[]
            {
                new ExcludeFromCodeCoverageClassHasBecauseAttribute(),
                new AllClassesNeedATest(),
                new TestClassesShouldMatchClassToTest(),
                new TestClassesShouldBePrivate()
            });
            // customized
            Conventions.AddRange(ConventionsFor<NZazuConventions>());
        }

        [Test]
        [TestCaseSource(nameof(TypesToTest))]
        public void Check(Type typeToTest)
        {
            Conventions.Check(typeToTest);
        }
    }
}