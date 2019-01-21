using System;
using System.Collections.Generic;
using NUnit.Framework;
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

        public static IFormCheck Create(IDictionary<string, string> settings)
        {
            Assert.Fail("Implement me");
            return null;

            //if (settings == null || settings.Count < 3) throw new ArgumentException("Hint source and target field needs to be specified");
            //if (settings.Count > 3) throw new ArgumentException("Only hint source and target field needs to be specified.");
            //return new GreaterThanFormCheck(settings[0].Value, settings[1], settings[2]);
        }
    }
}