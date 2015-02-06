using System;
using System.Collections.Generic;
using System.Linq;
using NZazu.Contracts.Checks;

namespace NZazu
{
    public static class ViewExtensions
    {
        public static Dictionary<string, string> GetFieldValues(this INZazuView view)
        {
            if (view == null) throw new ArgumentNullException("view");
            return view.FormDefinition.Fields.ToDictionary(f => f.Key, f => view.GetField(f.Key).StringValue);
        }

        public static void SetFieldValues(this INZazuView view, IEnumerable<KeyValuePair<string, string>> fieldValues)
        {
            if (view == null) throw new ArgumentNullException("view");
            if (fieldValues == null) throw new ArgumentNullException("fieldValues");
            foreach (var kvp in fieldValues)
                view.GetField(kvp.Key).StringValue = kvp.Value;
        }

        public static bool IsValid(this INZazuView view)
        {
            try
            {
                view.Validate();
                return true;
            }
            catch (ValidationException)
            {
                return false;
            }
        }
    }

    public static class FieldExtensions
    {
        public static bool IsValid(this INZazuField field)
        {
            try
            {
                field.Validate();
                return true;
            }
            catch (ValidationException)
            {
                return false;
            }
        }

    }
}