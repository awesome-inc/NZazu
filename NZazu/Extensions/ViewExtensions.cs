﻿using System;
using System.Collections.Generic;
using NZazu.Contracts.Checks;

namespace NZazu.Extensions
{
    public static class ViewExtensions
    {
        public static void SetFieldValues(this INZazuWpfView view, IEnumerable<KeyValuePair<string, string>> fieldValues)
        {
            if (view == null) throw new ArgumentNullException("view");
            if (fieldValues == null) throw new ArgumentNullException("fieldValues");
            foreach (var kvp in fieldValues)
                view.GetField(kvp.Key).StringValue = kvp.Value;
        }

        public static bool IsValid(this INZazuWpfView view)
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
}