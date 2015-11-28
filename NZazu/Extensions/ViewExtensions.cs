using System;
using System.Collections.Generic;

namespace NZazu.Extensions
{
    public static class ViewExtensions
    {
        public static void SetFieldValues(this INZazuWpfView view, IEnumerable<KeyValuePair<string, string>> fieldValues)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            if (fieldValues == null) throw new ArgumentNullException(nameof(fieldValues));
            foreach (var kvp in fieldValues)
            {
                try
                {
                    var field = view.GetField(kvp.Key);
                    if (field == null) continue;
                    field.StringValue = kvp.Value;
                }
                catch (KeyNotFoundException) { }
            }
        }

        public static bool IsValid(this INZazuWpfView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            return view.Validate().IsValid;
        }
    }
}