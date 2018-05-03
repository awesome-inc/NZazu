using System;
using System.Collections.Generic;
using System.Diagnostics;
using NZazu.Contracts;
using NZazu.FieldBehavior;
using NZazu.Serializer;

namespace NZazu.Fields
{
    public class NZazuFieldFactory : INZazuWpfFieldFactory
    {
        public INZazuWpfFieldBehaviorFactory BehaviorFactory { get; }
        public ICheckFactory CheckFactory { get; }
        public INZazuTableDataSerializer Serializer { get; }
        public INZazuWpfView View { get; set; }

        protected readonly Dictionary<string, Type> FieldTypes = new Dictionary<string, Type>();
        private const string DefaultType = "label";

        public NZazuFieldFactory(
            INZazuWpfFieldBehaviorFactory behaviorFactory = null,
            ICheckFactory checkFactory = null,
            INZazuTableDataSerializer serializer = null)
        {
            BehaviorFactory = behaviorFactory ?? new NZazuFieldBehaviorFactory();
            CheckFactory = checkFactory ?? new CheckFactory();
            Serializer = serializer ?? new NZazuTableDataXmlSerializer();

            FieldTypes.Add("label", typeof(NZazuLabelField));
            FieldTypes.Add("string", typeof(NZazuTextField));
            FieldTypes.Add("bool", typeof(NZazuBoolField));
            FieldTypes.Add("int", typeof(NZazuIntegerField));
            FieldTypes.Add("date", typeof(NZazuDateField));
            FieldTypes.Add("double", typeof(NZazuDoubleField));
            FieldTypes.Add("group", typeof(NZazuGroupField));
            FieldTypes.Add("datatable", typeof(NZazuDataTableField));
            FieldTypes.Add("option", typeof(NZazuOptionsField));
            FieldTypes.Add("keyedoption", typeof(NZazuKeyedOptionsField));
            FieldTypes.Add("imageViewer", typeof(NZazuImageViewerField));
        }

        public INZazuWpfField CreateField(FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));
            var fieldTypeSafe = fieldDefinition.Type ?? DefaultType;

            NZazuField field;
            if (FieldTypes.ContainsKey(fieldTypeSafe))
                field = (NZazuField)Activator.CreateInstance(FieldTypes[fieldTypeSafe], fieldDefinition);
            else
            {
                Trace.TraceWarning("The specified field type is not supported: " + fieldTypeSafe);
                var res = ProofForAvailableDescription(fieldDefinition);
                field = (NZazuField)Activator.CreateInstance(FieldTypes[DefaultType], res);
            }

            var safeField = field as IRequireFactory;
            if (safeField != null)
                safeField.FieldFactory = this;

            // TODO refactoring stuff ....
            var fieldContainer = field as INZazuWpfFieldContainer;
            fieldContainer?.CreateChildControls(this, fieldDefinition);

            return field
                .DecorateLabels(fieldDefinition)
                .ApplySettings(fieldDefinition)
                .AddOptionValues(fieldDefinition)
                .AddChecks(fieldDefinition.Checks, CheckFactory)
                .AddBehavior(fieldDefinition.Behavior, BehaviorFactory, View);
        }

        /// <summary>
        /// Some unknown types which are mapped to "label" does not carry a "Description" with them.
        /// The "Description" is taken as content for "label". If "Description" is not set you run into a System.NullReference exception otherwise.
        /// <see cref="NZazuLabelField.CreateValueControl()"/>
        /// </summary>
        private static FieldDefinition ProofForAvailableDescription(FieldDefinition fieldDefinition)
        {
            fieldDefinition.Description = fieldDefinition.Description ?? "-";
            return fieldDefinition;
        }
    }
}