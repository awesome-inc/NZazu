using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    public class NZazuFieldFactory : INZazuWpfFieldFactory
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
            FieldTypes.Add("double", typeof(NZazuDoubleField));
            FieldTypes.Add("group", typeof(NZazuGroupField));
        }

        public INZazuWpfField CreateField(FieldDefinition fieldDefinition)
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

            field.Check = CreateCheck(fieldDefinition.Checks);

            CopySettings(field, fieldDefinition);

            RecurseGroupField(fieldDefinition, field as NZazuGroupField);

            return field;
        }

        private static void CopySettings(NZazuField field, FieldDefinition fieldDefinition)
        {
            if (fieldDefinition.Settings != null)
            {
                foreach (var kvp in fieldDefinition.Settings)
                    field.Settings[kvp.Key] = kvp.Value;
            }
        }

        private IValueCheck CreateCheck(IEnumerable<CheckDefinition> checkDefinitions)
        {
            if (checkDefinitions == null) return null;
 
            var checks = checkDefinitions.Select(c => _checkFactory.CreateCheck(c)).ToArray();
            return checks.Length == 1 
                ? checks.First() 
                : new AggregateCheck(checks.ToArray());
        }

        private void RecurseGroupField(FieldDefinition fieldDefinition, NZazuGroupField groupField)
        {
            if (fieldDefinition == null) throw new ArgumentNullException("fieldDefinition");
            if (groupField == null) return;
            if (fieldDefinition.Fields == null || !fieldDefinition.Fields.Any()) return;

            groupField.Fields = fieldDefinition.Fields.Select(CreateField);
        }
    }
}