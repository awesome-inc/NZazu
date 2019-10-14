using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    public static class FieldFactoryExtensions
    {
        public static NZazuField ApplySettings(this NZazuField field, FieldDefinition fieldDefinition)
        {
            if (fieldDefinition.Settings == null) return field;

            var control = field.ValueControl;
            var height = fieldDefinition.Settings.Get<double>("Height");
            if (height.HasValue)
                control.MinHeight = control.MaxHeight = height.Value;

            var width = fieldDefinition.Settings.Get<double>("Width");
            if (width.HasValue)
                control.MinWidth = control.MaxWidth = width.Value;

            // apply generic settings
            var controlSettings = fieldDefinition.Settings.Where(s => control.CanSetProperty(s.Key));

            try
            {
                foreach (var setting in controlSettings)
                    control.SetProperty(setting.Key, setting.Value);
            }
            catch (ArgumentException exception)
            {
                Trace.TraceError(
                    $"Following error occured while setting the properties for the field <{field.Key}>: {Environment.NewLine}{exception}");
            }

            return field;
        }

        public static NZazuField AddOptionValues(this NZazuField field, FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));
            if (fieldDefinition.Values == null || !fieldDefinition.Values.Any()) return field;

            switch (field)
            {
                case NZazuOptionsField optionsField:
                    optionsField.Options = fieldDefinition.Values.ToArray();
                    break;
                case NZazuKeyedOptionsField keyedOptionField:
                    keyedOptionField.Options = fieldDefinition.Values.ToArray();
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

            var checks = checkDefinitions
                .Select(x => checkFactory.CreateCheck(x, field.Definition, formData, tableSerializer, rowIdx))
                .ToArray();
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
            var behaviors = new List<INZazuWpfFieldBehavior>();
            foreach (var behaviorDefinition in behaviorDefinitions)
            {
                var behavior = behaviorFactory.CreateFieldBehavior(behaviorDefinition);

                behavior?.AttachTo(field, view);
                behaviors.Add(behavior);
            }

            field.Behaviors = behaviors.ToArray();

            return field;
        }
    }
}