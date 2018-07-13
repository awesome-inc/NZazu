using System;
using NZazu.Contracts.Checks;
using NZazu.Contracts.FormChecks;

namespace NZazu.Contracts
{
    public interface ICheckFactory
    {
        IValueCheck CreateCheck(CheckDefinition checkDefinition, Func<FormData> formData = null,
            INZazuTableDataSerializer tableSerializer = null);

        IFormCheck CreateFormCheck(CheckDefinition checkDefinition);
    }
}