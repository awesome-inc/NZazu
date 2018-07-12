using System;
using NZazu.Contracts.Checks;

namespace NZazu.Contracts.FormChecks
{
    public class GreaterThanFormCheck : IFormCheck
    {
        private readonly string _leftFieldName;
        private readonly string _rightFieldName;
        private readonly string _hint;


        public GreaterThanFormCheck(string hint, string leftFieldName, string rightFieldName)
        {
            _leftFieldName = leftFieldName ?? throw new ArgumentNullException(nameof(leftFieldName));
            _rightFieldName = rightFieldName ?? throw new ArgumentNullException(nameof(rightFieldName));
            _hint = hint ?? string.Empty;
        }

        public ValueCheckResult Validate(FormData formData, IFormatProvider formatProvider = null)
        {
            var leftFieldValue = formData.Values?[_leftFieldName];
            var rightFieldValue = formData.Values?[_rightFieldName];

            long.TryParse(leftFieldValue, out var leftNumber);
            long.TryParse(rightFieldValue, out var rightNumber);

            return leftNumber < rightNumber ? 
                new ValueCheckResult(false, new ArgumentException(_hint)) 
                : ValueCheckResult.Success;
        }
    }
}