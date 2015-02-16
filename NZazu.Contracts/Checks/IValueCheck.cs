using System;

namespace NZazu.Contracts.Checks
{
    public interface IValueCheck
    {
        ValueCheckResult Validate(string value, IFormatProvider formatProvider = null);
    }
}