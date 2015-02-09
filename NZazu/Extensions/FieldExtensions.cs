using NZazu.Contracts.Checks;

namespace NZazu.Extensions
{
    public static class FieldExtensions
    {
        public static bool IsValid(this INZazuWpfField field)
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