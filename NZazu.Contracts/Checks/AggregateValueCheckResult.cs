using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts.Checks
{
    /// <summary>
    ///     Implements an aggregation of <see cref="ValueCheckResult" />.
    ///     The errors are merged to an <see cref="AggregateException" />.
    ///     The messages are joins to one single message.
    /// </summary>
    public class AggregateValueCheckResult : ValueCheckResult
    {
        public AggregateValueCheckResult(IEnumerable<ValueCheckResult> checkResults)
        {
            var checkResultsArray = checkResults.ToArray();

            // valid only when ALL results are valid
            IsValid = checkResultsArray.All(x => x.IsValid);

            var message = string.Join(", ", checkResultsArray
                .Where(x => !x.IsValid)
                .Select(x => x.Exception.Message));

            Exception = new AggregateException(message, checkResultsArray
                .Where(x => !x.IsValid)
                .Select(x => x.Exception));
        }
    }
}