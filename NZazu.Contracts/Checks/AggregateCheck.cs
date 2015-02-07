using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NZazu.Contracts.Checks
{
    public class AggregateCheck : IValueCheck
    {
        private readonly List<IValueCheck> _checks;
        public IEnumerable<IValueCheck> Checks { get { return _checks; } }


        public AggregateCheck(params IValueCheck[] checks)
        {
            _checks = (checks ?? Enumerable.Empty<IValueCheck>()).ToList();
        }

        public void Validate(string value, CultureInfo cultureInfo)
        {
            _checks.ForEach(c => c.Validate(value, cultureInfo));
        }
    }
}