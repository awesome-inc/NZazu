using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts.Checks
{
    public class AggregateCheck : IValueCheck
    {
        private readonly List<IValueCheck> _checks;


        public AggregateCheck(IEnumerable<IValueCheck> checks)
        {
            _checks = (checks ?? Enumerable.Empty<IValueCheck>()).ToList();
        }

        public IEnumerable<IValueCheck> Checks => _checks;

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider)
        {
            var invalidChecks = _checks
                .Select(c => c.Validate(value, parsedValue, formatProvider) ?? ValueCheckResult.Success)
                .Where(x => !x.IsValid)
                .ToArray();

            switch (invalidChecks.Length)
            {
                case 0: return ValueCheckResult.Success;
                case 1: return invalidChecks.First();
                default: return new AggregateValueCheckResult(invalidChecks);
            }
        }
    }
}