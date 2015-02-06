using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NZazu.Contracts;
using NZazu.Fields;

namespace NZazu.Factory
{
    class NZazuFieldFactory : INZazuFieldFactory
    {
        private readonly Dictionary<string, Type> _fieldTypes = new Dictionary<string, Type>();
        private const string DefaultType = "label";

        public NZazuFieldFactory()
        {
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

        private static NZazuField Decorate(NZazuField field, FieldDefinition fieldDefinition)
        {
            field.Prompt = fieldDefinition.Prompt;
            field.Hint = fieldDefinition.Hint;
            field.Description = fieldDefinition.Description;
            field.Checks = fieldDefinition.Checks;
            field.Settings = fieldDefinition.Settings ?? new Dictionary<string, string>();
            return field;
        }
    }
}