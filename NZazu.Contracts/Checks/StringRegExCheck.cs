using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NZazu.Contracts.Checks
{
    public class StringRegExCheck : IValueCheck
    {
        private readonly string _hint;
        private readonly Regex[] _regex;

        public StringRegExCheck(string hint, params Regex[] regex)
        {
            if (String.IsNullOrWhiteSpace(hint)) throw new ArgumentNullException("hint");
            if (regex == null) throw new ArgumentNullException("regex");
            if (!regex.Any()) throw new ArgumentException("No regex patterns specified");
            _hint = hint;
            _regex = regex;
        }

        public void Validate(string value, IFormatProvider formatProvider = null)
        {
            if (value == null) throw new ValidationException(_hint);
            var anyMatch = _regex.Any(r => r.IsMatch(value));
            if (!anyMatch) throw new ValidationException(_hint);
        }
    }
}