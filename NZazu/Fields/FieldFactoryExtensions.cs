using System;
using System.Collections.Generic;
using System.Linq;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    public static class FieldFactoryExtensions
    {
        public static NZazuField DecorateLabels(this NZazuField field, FieldDefinition fieldDefinition)
        {
            field.Prompt = fieldDefinition.Prompt;
            field.Hint = fieldDefinition.Hint;
            field.Description = fieldDefinition.Description;

            return field;
        }

        public static NZazuField ApplySettings(this NZazuField field, FieldDefinition fieldDefinition)
        {
            if (fieldDefinition.Settings == null) return field;

            foreach (var kvp in fieldDefinition.Settings)
                field.Settings[kvp.Key] = kvp.Value;

            return field;
        }

        public static NZazuField AddOptionValues(this NZazuField field, FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));
            if (fieldDefinition.Values == null || !fieldDefinition.Values.Any()) return field;

            switch (field)
            {
                case NZazuOptionsField optionsField:
                    optionsField.Options = fieldDefinition.Values;
                    break;
                case NZazuKeyedOptionsField keyedOptionField:
                    keyedOptionField.Options = fieldDefinition.Values;
                    break;
                default:
                    return field;
            }

            return field;
        }

        public static NZazuField AddChecks(
            this NZazuField field,
            IEnumerable<CheckDefinition> checkDefinitions,
            ICheckFactory checkFactory,
            Func<FormData> formData = null,
            INZazuTableDataSerializer tableSerializer = null,
            int rowIdx = -1)
        {
            if (checkDefinitions == null) return field;

            var checks = checkDefinitions.Select(checkdef => checkFactory.CreateCheck(checkdef, formData, tableSerializer, rowIdx)).ToArray();
            field.Check = checks.Length == 1
                ? checks.First()
                : new AggregateCheck(checks.ToArray());

            return field;
        }

        public static NZazuField AddBehaviors(
            this NZazuField field,
            IEnumerable<BehaviorDefinition> behaviorDefinitions,
            INZazuWpfFieldBehaviorFactory behaviorFactory,
            INZazuWpfView view)
        {
            if (behaviorDefinitions == null) return field;

            // add behaviors
            foreach (var behaviorDefinition in behaviorDefinitions)
            {
                var behavior = behaviorFactory.CreateFieldBehavior(behaviorDefinition);

                behavior?.AttachTo(field, view);
                field.Behaviors.Add(behavior);
            }

            return field;
        }
    }

}