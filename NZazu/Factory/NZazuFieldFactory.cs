using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Fields;

namespace NZazu.Factory
{
    class NZazuFieldFactory : INZazuFieldFactory
    {
        private readonly ICheckFactory _checkFactory;
        private readonly Dictionary<string, Type> _fieldTypes = new Dictionary<string, Type>();
        private const string DefaultType = "label";

        public NZazuFieldFactory(ICheckFactory checkFactory = null)
        {
            _checkFactory = checkFactory ?? new CheckFactory();

            _fieldTypes.Add("label", typeof(NZazuLabelField));
            _fieldTypes.Add("string", typeof(NZazuTextField));
            _fieldTypes.Add("bool", typeof(NZazuBoolField));
            _fieldTypes.Add("int", typeof(NZazuIntegerField));
            _fieldTypes.Add("date", typeof(NZazuDateField));
            _fieldTypes.Add("double", typeof(NZazuDoubleField));
        }

        public INZazuField CreateField(FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException("fieldDefinition");
            var fieldTypeSafe = fieldDefinition.Type ?? DefaultType;

            NZazuField field;
            if (_fieldTypes.ContainsKey(fieldTypeSafe))
                field = (NZazuField)Activator.CreateInstance(_fieldTypes[fieldTypeSafe], fieldDefinition.Key);
            else
            {
                Trace.TraceWarning("The specified field type is not supported: " + fieldTypeSafe);
                field = (NZazuField)Activator.CreateInstance(_fieldTypes[DefaultType], fieldDefinition.Key);
            }

            return Decorate(field, fieldDefinition);
        }

        private NZazuField Decorate(NZazuField field, FieldDefinition fieldDefinition)
        {
            field.Prompt = fieldDefinition.Prompt;
            field.Hint = fieldDefinition.Hint;
            field.Description = fieldDefinition.Description;
            field.Checks = CreateChecks(fieldDefinition.Checks);
            return field;
        }

        private IEnumerable<IValueCheck> CreateChecks(IEnumerable<CheckDefinition> checkDefinitions)
        {
            return checkDefinitions == null 
                ? Enumerable.Empty<IValueCheck>() 
                : checkDefinitions.Select(c => _checkFactory.CreateCheck(c)).ToArray();
        }
    }
}