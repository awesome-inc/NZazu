using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NZazu.Contracts.Checks;
using NZazu.Contracts.FormChecks;

namespace NZazu.Contracts
{
    public class CheckFactory : ICheckFactory
    {
        private IDictionary<string, Type> _registrations = new Dictionary<string, Type>();

        public CheckFactory()
        {
            RegisterCurrentAssemblyValueChecks();
        }

        public IEnumerable<string> AvailableTypes => _registrations.Keys;

        public IValueCheck CreateCheck(
            CheckDefinition checkDefinition,
            FieldDefinition fieldDefinition,
            Func<FormData> formData = null,
            INZazuTableDataSerializer tableSerializer = null,
            int rowIdx = -1)
        {
            if (checkDefinition == null) throw new ArgumentNullException(nameof(checkDefinition));
            if (string.IsNullOrWhiteSpace(checkDefinition.Type))
                throw new ArgumentException("check type not specified");

            // lets check if the given validation type is available
            if (!_registrations.ContainsKey(checkDefinition.Type.ToLower()))
                throw new NotSupportedException($"The specified check '{checkDefinition.Type}' is not supported");

            var concrete = _registrations[checkDefinition.Type.ToLower()];
            Trace.WriteLine($"Found check for '{checkDefinition.Type.ToLower()}' as ({concrete.FullName})");

            // lets use reflection to create an instance
            var result = (IValueCheck)
                Activator.CreateInstance(concrete, checkDefinition.Settings, formData, tableSerializer, rowIdx,
                    fieldDefinition);

            return result;
        }

        public IFormCheck CreateFormCheck(CheckDefinition checkDefinition)
        {
            if (checkDefinition == null) throw new ArgumentNullException(nameof(checkDefinition));
            if (string.IsNullOrWhiteSpace(checkDefinition.Type))
                throw new ArgumentException("form check type not specified");
            switch (checkDefinition.Type)
            {
                case "gt": return new GreaterThanFormCheck(checkDefinition.Settings);
                default: throw new NotSupportedException("The specified check is not supported");
            }
        }

        private void RegisterCurrentAssemblyValueChecks()
        {
            _registrations = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.GetInterface(typeof(IValueCheck).Name) != null && x != typeof(AggregateCheck))
                .ToDictionary(
                    GetClassDisplayName,
                    x => x);

            var types = string.Join(",", AvailableTypes);
            Trace.WriteLine($"[CheckFactory] Available check types: {types}");
        }

        private static string GetClassDisplayName(Type cls)
        {
            var nameAttribute = cls.GetCustomAttribute<DisplayNameAttribute>();

            return nameAttribute != null
                ? nameAttribute.DisplayName.ToLower()
                : cls.Name.ToLower();
        }
    }
}