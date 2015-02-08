using System;

namespace NZazu.Contracts.Checks
{
    public interface IValueCheck
    {
        /// <exception cref="ValidationException"/>
        void Validate(string value, IFormatProvider formatProvider = null);
    }
}