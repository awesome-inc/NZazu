using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Fields;

namespace NZazu.Factory
{
    public class NZazuFieldFactory : INZazuFieldFactory
    {
        private readonly ICheckFactory _checkFactory;
        protected readonly Dictionary<string, Type> FieldTypes = new Dictionary<string, Type>();
        private const string DefaultType = "label";

        public NZazuFieldFactory(ICheckFactory checkFactory = null)
        {
            _checkFactory = checkFactory ?? new CheckFactory();

            FieldTypes.Add("label", typeof(NZazuLabelField));
            FieldTypes.Add("string", typeof(NZazuTextField));
            FieldTypes.Add("bool", typeof(NZazuBoolField));
            FieldTypes.Add("int", typeof(NZazuIntegerField));
            FieldTypes.Add("date", typeof(NZazuDateField));
        }

        public INZazuField CreateField(FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException("fieldDefinition");
            var fieldTypeSafe = fieldDefinition.Type ?? DefaultType;

            NZazuField field;
            if (FieldTypes.ContainsKey(fieldTypeSafe))
                field = (NZazuField)Activator.CreateInstance(FieldTypes[fieldTypeSafe], fieldDefinition.Key);
            else
            {
                Trace.TraceWarning("The specified field type is not supported: " + fieldTypeSafe);
                field = (NZazuField)Activator.CreateInstance(FieldTypes[DefaultType], fieldDefinition.Key);
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