namespace NZazu.Contracts.Checks
{
    public class ValueCheckResult
    {
        public static readonly ValueCheckResult Success = new ValueCheckResult(true);

        public bool IsValid { get; private set; }
        public object Error { get; private set; }

        public ValueCheckResult(bool isValid, object error = null)
        {
            IsValid = isValid;
            Error = error;
        }
    }
}