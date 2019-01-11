using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts.Checks
{
    public class AggregateCheck : IValueCheck
    {
        private readonly List<IValueCheck> _checks;
        public IEnumerable<IValueCheck> Checks => _checks;


        public AggregateCheck(params IValueCheck[] checks)
        {
            _checks = (checks ?? Enumerable.Empty<IValueCheck>()).ToList();
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider)
        {
            var invalidChecks = _checks
                .Select(c => c.Validate(value, parsedValue, formatProvider))
                .Where(x => !x.IsValid)
                .ToArray();

            return invalidChecks.Length == 0 
                ? ValueCheckResult.Success 
                : new AggregateValueCheckResult(invalidChecks);
        }
    }
}