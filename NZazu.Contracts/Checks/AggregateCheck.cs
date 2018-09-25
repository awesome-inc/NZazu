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
            var invalid = _checks.Select(c => c.Validate(value, parsedValue, formatProvider)).FirstOrDefault(vr => !vr.IsValid);
            return invalid ?? ValueCheckResult.Success;
        }
    }
}