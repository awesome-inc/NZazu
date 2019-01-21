using NZazu.Contracts.Checks;
using NZazu.Contracts.FormChecks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace NZazu.Contracts
{
    public class CheckFactory : ICheckFactory
    {
        private IDictionary<string, Type> _registrations = new Dictionary<string, Type>();

        public CheckFactory()
        {
            RegisterAssemblyValueChecks();
        }

        private void RegisterAssemblyValueChecks()
        {
            _registrations = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.GetInterface(typeof(IValueCheck).Name) != null)
                .ToDictionary(
                    GetClassDisplayName,
                    x => x);
        }

        private static string GetClassDisplayName(Type cls)
        {
            var nameAttribute = cls.GetCustomAttribute<DisplayNameAttribute>();

            return nameAttribute != null 
                ? nameAttribute.DisplayName.ToLower()
                : cls.Name.ToLower();
        }

        public IValueCheck CreateCheck(CheckDefinition checkDefinition, Func<FormData> formData = null,
            INZazuTableDataSerializer tableSerializer = null, int rowIdx = -1)
        {
            if (checkDefinition == null) throw new ArgumentNullException(nameof(checkDefinition));
            if (string.IsNullOrWhiteSpace(checkDefinition.Type)) throw new ArgumentException("check type not specified");

            var concrete = _registrations.ContainsKey(checkDefinition.Type.ToLower())
                ? _registrations[checkDefinition.Type.ToLower()]
                : throw new NotSupportedException($"The specified check '{checkDefinition.Type}' is not supported");

            var result = (IValueCheck)
                Activator.CreateInstance(concrete, checkDefinition.Settings, formData, tableSerializer, rowIdx);

            return result;
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