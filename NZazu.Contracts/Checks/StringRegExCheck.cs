using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NZazu.Contracts.Checks
{
    public class StringRegExCheck : IValueCheck
    {
        private readonly Regex[] _regex;
        private readonly ValueCheckResult _noMatch;

        public StringRegExCheck(string hint, params Regex[] regex)
        {
            if (string.IsNullOrWhiteSpace(hint)) throw new ArgumentNullException(nameof(hint));
            if (regex == null) throw new ArgumentNullException(nameof(regex));
            if (!regex.Any()) throw new ArgumentException("No regex patterns specified");
            _regex = regex;

            _noMatch = new ValueCheckResult(false, new ArgumentException(hint));
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            if (string.IsNullOrWhiteSpace(value)) return ValueCheckResult.Success;
            var anyMatch = _regex.Any(r => r.IsMatch(value));
            return anyMatch ? ValueCheckResult.Success : _noMatch;
        }
    }
}