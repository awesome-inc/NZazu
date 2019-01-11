using System;

namespace NZazu.Contracts.Checks
{
    public class ValueCheckResult
    {
        public static readonly ValueCheckResult Success = new ValueCheckResult(true);

        public bool IsValid { get; }
        public Exception Error { get; }

        public ValueCheckResult(bool isValid, Exception error = null)
        {
            IsValid = isValid;
            Error = error;
        }

        public ValueCheckResult(Exception error = null) : this(error == null, error) { }
    }
}