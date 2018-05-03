using System;
using NEdifis.Attributes;

namespace NZazu.Fields
{
    [ExcludeFromConventions("this is a simple DTO")]
    internal class ValueChangedEventArgs<T>
    {
        public string StoreKey { get; }
        public Guid CtrlGuid { get; }
        public T OldValue { get; }
        public T NewValue { get; }

        public ValueChangedEventArgs(string storeKey, Guid ctrlGuid, T oldValue, T newValue)
        {
            StoreKey = storeKey;
            CtrlGuid = ctrlGuid;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}