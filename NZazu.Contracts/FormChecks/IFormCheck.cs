using System;
using NZazu.Contracts.Checks;

namespace NZazu.Contracts.FormChecks
{
    public interface IFormCheck
    {
        ValueCheckResult Validate(FormData formData, IFormatProvider formatProvider = null);
    }
}