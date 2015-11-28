namespace NZazu.Contracts.Checks
{
    public class ValueCheckResult
    {
        public static readonly ValueCheckResult Success = new ValueCheckResult(true);

        public bool IsValid { get; }
        public object Error { get; }

        public ValueCheckResult(bool isValid, object error = null)
        {
            IsValid = isValid;
            Error = error;
        }
    }
}