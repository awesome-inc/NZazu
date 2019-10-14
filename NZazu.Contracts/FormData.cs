using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts
{
    // NOTE: Equals should have GetHashCode. However, we do not want to use FormData as keys in Dictionaries etc.
#pragma warning disable 659
    public class FormData : IEquatable<FormData>
    {
        public FormData(IDictionary<string, string> values = null)
        {
            Values = values?.ToDictionary(v => v.Key, v => v.Value) ?? new Dictionary<string, string>();
        }

        public Dictionary<string, string> Values { get; }

        public bool Equals(FormData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Values.Equivalent(other.Values);
        }

        public static implicit operator FormData(Dictionary<string, string> values)
        {
            return new FormData(values.Remove(kvp => kvp.Value == null));
        }

        public static implicit operator Dictionary<string, string>(FormData formData)
        {
            return formData.Values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FormData) obj);
        }
    }
}