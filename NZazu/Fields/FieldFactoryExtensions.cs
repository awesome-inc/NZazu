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

            var optionsField = field as NZazuOptionsField;
            if (optionsField == null) return field;
            optionsField.Options = fieldDefinition.Values;

            return field;
        }

        public static NZazuField AddChecks(this NZazuField field, IEnumerable<CheckDefinition> checkDefinitions, ICheckFactory checkFactory, Func<FormData> formData = null, INZazuTableDataSerializer tableSerializer = null)
        {
            if (checkDefinitions == null) return field;

            var checks = checkDefinitions.Select(checkdef => checkFactory.CreateCheck(checkdef, formData)).ToArray();
            field.Check = checks.Length == 1
                ? checks.First()
                : new AggregateCheck(checks.ToArray());

            return field;
        }

        public static NZazuField AddBehavior(
            this NZazuField field, BehaviorDefinition behaviorDefinition,
            INZazuWpfFieldBehaviorFactory behaviorFactory, INZazuWpfView view)
        {
            if (behaviorDefinition == null) return field;

            // add behavior
            var behavior = behaviorFactory.CreateFieldBehavior(behaviorDefinition);
            if (behavior == null) return field;

            behavior.AttachTo(field, view);
            field.Behavior = behavior;

            return field;
        }
    }
}