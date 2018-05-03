using NEdifis.Attributes;

namespace NZazu.Fields
{
    [ExcludeFromConventions("this is a simple DTO")]
    internal class ValueChangedEventArgs<T>
    {
        public string StoreKey { get; }
        public T OldValue { get; }
        public T NewValue { get; }

        public ValueChangedEventArgs(string storeKey, T oldValue, T newValue)
        {
            StoreKey = storeKey;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}