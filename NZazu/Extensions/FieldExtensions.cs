using System;

namespace NZazu.Extensions
{
    public static class FieldExtensions
    {
        public static bool IsValid(this INZazuWpfField field)
        {
            if (field == null) throw new ArgumentNullException("field");
            return field.Validate().IsValid;
        }
    }
}