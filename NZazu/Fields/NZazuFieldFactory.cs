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
        public INZazuWpfView View { get; set; }

        protected readonly IDictionary<string, Type> FieldTypes = new Dictionary<string, Type>();
        private readonly IDictionary<Type, object> Factorys = new Dictionary<Type, object>();
        private const string DefaultType = "default";

        public NZazuFieldFactory(
            INZazuWpfFieldBehaviorFactory behaviorFactory = null,
            ICheckFactory checkFactory = null,
            INZazuTableDataSerializer serializer = null)
        {
            Factorys.Add(typeof(INZazuWpfFieldFactory), this);
            Factorys.Add(typeof(INZazuWpfFieldBehaviorFactory), behaviorFactory ?? new NZazuFieldBehaviorFactory());
            Factorys.Add(typeof(ICheckFactory), checkFactory ?? new CheckFactory());
            Factorys.Add(typeof(INZazuTableDataSerializer), serializer ?? new NZazuTableDataXmlSerializer());

            // we add label twice to have it as default type
            FieldTypes.Add(DefaultType, typeof(NZazuLabelField));

            // lets add all nzazu core fields and types
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
            FieldTypes.Add("location", typeof(NZazuLocationField));
        }

        public INZazuWpfField CreateField(FieldDefinition fieldDefinition, int rowIdx = -1)
        {
            var behaviourFactory = (INZazuWpfFieldBehaviorFactory)Factorys[typeof(INZazuWpfFieldBehaviorFactory)];
            var checkFactory = (ICheckFactory)Factorys[typeof(ICheckFactory)];
            var serializer = (INZazuTableDataSerializer)Factorys[typeof(INZazuTableDataSerializer)];

            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));

            // we throw an exception but this should not happen by default and only if the user adds null
            if (behaviourFactory == null) throw new ArgumentNullException(nameof(behaviourFactory));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (checkFactory == null) throw new ArgumentNullException(nameof(checkFactory));

#pragma warning disable 618
            return CreateFieldInstance(fieldDefinition)
                .Initialize(Resolve<object>)
                .DecorateLabels(fieldDefinition)
                .ApplySettings(fieldDefinition)
                .AddOptionValues(fieldDefinition)
                .AddBehaviors(fieldDefinition.Behaviors, behaviourFactory, View)
                .AddChecks(fieldDefinition.Checks, checkFactory, View != null ? () => View.FormData : (Func<FormData>)null, serializer, rowIdx);
#pragma warning restore 618
        }

        public T Resolve<T>(Type x = null)
        {
            if (x == null) x = typeof(T);

            return (T)(Factorys.ContainsKey(x) ? Factorys[x] : null);
        }

        private NZazuField CreateFieldInstance(FieldDefinition fieldDefinition)
        {
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

            return field;
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