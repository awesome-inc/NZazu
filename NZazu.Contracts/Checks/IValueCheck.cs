using System.Globalization;

namespace NZazu.Contracts.Checks
{
    public interface IValueCheck
    {
        /// <exception cref="ValidationException"/>
        void Validate(string value, CultureInfo cultureInfo = null);
    }
}