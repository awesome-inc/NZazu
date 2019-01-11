using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts.Checks
{
    public class AggregateValueCheckResult : ValueCheckResult
    {
        public AggregateValueCheckResult(IEnumerable<ValueCheckResult> checkResults)
        {
            var checkResultsArray = checkResults.ToArray();

            IsValid = checkResultsArray.Any(x => !x.IsValid);

            var message = string.Join(", ", checkResultsArray.Select(x => x.Error.Message));
            Error = new AggregateException(message, checkResultsArray.Select(x => x.Error));
        }

    }
}