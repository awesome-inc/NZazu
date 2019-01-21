using NZazu.Contracts.Checks;
using NZazu.Contracts.FormChecks;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NZazu.Contracts
{
    public class CheckFactory : ICheckFactory
    {
        public CheckFactory()
        {
            RegisterAssemblyValueChecks();
        }

        private void RegisterAssemblyValueChecks()
        {
            var registrations = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.GetInterface(typeof(IValueCheck).Name) != null)
                .ToDictionary(
                    GetClassDisplayName,
                    x => x);

        }

        private string GetClassDisplayName(Type cls)
        {
            var nameAttribute = cls.GetCustomAttribute<DisplayNameAttribute>();

            return nameAttribute != null 
                ? nameAttribute.DisplayName 
                : cls.Name;
        }

        public IValueCheck CreateCheck(CheckDefinition checkDefinition, Func<FormData> formData = null,
            INZazuTableDataSerializer tableSerializer = null, int rowIdx = -1)
        {
            if (checkDefinition == null) throw new ArgumentNullException(nameof(checkDefinition));
            if (string.IsNullOrWhiteSpace(checkDefinition.Type)) throw new ArgumentException("check type not specified");
            switch (checkDefinition.Type)
            {
                case "required": return RequiredCheck.Create(checkDefinition.Settings);
                case "length": return StringLengthCheck.Create(checkDefinition.Settings);
                case "range": return RangeCheck.Create(checkDefinition.Settings);
                case "regex": return StringRegExCheck.Create(checkDefinition.Settings);
                case "dateTime": return DateTimeComparisonCheck.Create(checkDefinition.Settings, formData, tableSerializer, rowIdx);
                default: throw new NotSupportedException("The specified check is not supported");
            }
        }

        public IFormCheck CreateFormCheck(CheckDefinition checkDefinition)
        {
            if (checkDefinition == null) throw new ArgumentNullException(nameof(checkDefinition));
            if (string.IsNullOrWhiteSpace(checkDefinition.Type)) throw new ArgumentException("form check type not specified");
            switch (checkDefinition.Type)
            {
                case "gt": return GreaterThanFormCheck.Create(checkDefinition.Settings);
                default: throw new NotSupportedException("The specified check is not supported");
            }
        }
    }
}