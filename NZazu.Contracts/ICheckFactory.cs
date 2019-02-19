using System;
using System.Collections.Generic;
using NZazu.Contracts.Checks;
using NZazu.Contracts.FormChecks;

namespace NZazu.Contracts
{
    public interface ICheckFactory
    {
        IValueCheck CreateCheck(
            CheckDefinition checkDefinition,
            Func<FormData> formData = null,
            INZazuTableDataSerializer tableSerializer = null, 
            int rowIdx = -1);

        IFormCheck CreateFormCheck(CheckDefinition checkDefinition);
        IEnumerable<string> AvailableTypes { get; }
    }
}