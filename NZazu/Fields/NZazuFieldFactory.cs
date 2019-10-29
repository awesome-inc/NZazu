using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using NZazu.Contracts;
using NZazu.Contracts.Adapter;
using NZazu.Contracts.Suggest;
using NZazu.Extensions;
using NZazu.FieldBehavior;
using NZazu.Serializer;

namespace NZazu.Fields
{
    public class NZazuFieldFactory : INZazuWpfFieldFactory
    {
        // ReSharper disable once InconsistentNaming
        private const string DefaultType = "default";
        private readonly IDictionary<Type, object> _serviceLocator = new Dictionary<Type, object>();

        protected readonly IDictionary<string, Type> FieldTypes = new Dictionary<string, Type>();

        public NZazuFieldFactory()
        {
            // lets add the default implementations to the factory
            _serviceLocator.Add(typeof(INZazuWpfFieldFactory), this);
            _serviceLocator.Add(typeof(INZazuWpfFieldBehaviorFactory), new NZazuFieldBehaviorFactory());
            _serviceLocator.Add(typeof(ICheckFactory), new CheckFactory());
            _serviceLocator.Add(typeof(INZazuTableDataSerializer), new NZazuTableDataXmlSerializer());
            _serviceLocator.Add(typeof(IFormatProvider), CultureInfo.InvariantCulture);
            _serviceLocator.Add(typeof(IValueConverter), NoExceptionsConverter.Instance);
            _serviceLocator.Add(typeof(ISupportGeoLocationBox), new SupportGeoLocationBox());
            _serviceLocator.Add(typeof(IProvideSuggestions), new AggregateProvideSuggestions(new IProvideSuggestions[]
            {
                new ProvideValueSuggestions(),
                new ProvideFileSuggestions()
            }));

            // lets add all NZazu core fields and types
            FieldTypes.Add(DefaultType, typeof(NZazuLabelField)); // we add label twice to have it as default type
            FieldTypes.Add("error", typeof(NZazuErrorListField));
            FieldTypes.Add("label", typeof(NZazuLabelField));
            FieldTypes.Add("string", typeof(NZazuTextField));
            FieldTypes.Add("multiline", typeof(NZazuMultilineField));
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
            FieldTypes.Add("autocomplete", typeof(NZazuAutocompleteField));
        }

        public INZazuWpfField CreateField(FieldDefinition fieldDefinition, int rowIdx = -1)
        {
            var behaviourFactory =
                (INZazuWpfFieldBehaviorFactory) _serviceLocator[typeof(INZazuWpfFieldBehaviorFactory)];
            var checkFactory = (ICheckFactory) _serviceLocator[typeof(ICheckFactory)];
            var serializer = (INZazuTableDataSerializer) _serviceLocator[typeof(INZazuTableDataSerializer)];
            _serviceLocator.TryGetValue(typeof(INZazuWpfView), out var view);

            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));

            // we throw an exception but this should not happen by default and only if the user adds null
            if (behaviourFactory == null) throw new ArgumentNullException(nameof(behaviourFactory));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (checkFactory == null) throw new ArgumentNullException(nameof(checkFactory));

#pragma warning disable 618
            return CreateFieldInstance(fieldDefinition)
                .ApplySettings(fieldDefinition)
                .AddOptionValues(fieldDefinition)
                .AddBehaviors(fieldDefinition.Behaviors, behaviourFactory, view as INZazuWpfView)
                .AddChecks(
                    fieldDefinition.Checks, checkFactory,
                    () => ((INZazuView) view)?.FormData ?? new Dictionary<string, string>(), serializer, rowIdx);
#pragma warning restore 618
        }

        public T Resolve<T>(Type x = null)
        {
            if (x == null) x = typeof(T);

            return (T) (_serviceLocator.ContainsKey(x) ? _serviceLocator[x] : null);
        }

        public void Use<T>(T service)
        {
            if (_serviceLocator.ContainsKey(typeof(T)))
                _serviceLocator[typeof(T)] = service;
            else
                _serviceLocator.Add(typeof(T), service);
        }

        private NZazuField CreateFieldInstance(FieldDefinition fieldDefinition)
        {
            var fieldTypeSafe = fieldDefinition.Type ?? DefaultType;

            NZazuField field;
            if (FieldTypes.ContainsKey(fieldTypeSafe))
            {
                field = (NZazuField) Activator.CreateInstance(FieldTypes[fieldTypeSafe], fieldDefinition,
                    (Func<Type, object>) Resolve<object>);
            }
            else
            {
                Trace.TraceWarning("The specified field type is not supported: " + fieldTypeSafe);
                var res = ProofForAvailableDescription(fieldDefinition);
                field = (NZazuField) Activator.CreateInstance(FieldTypes[DefaultType], res,
                    (Func<Type, object>) Resolve<object>);
            }

            return field;
        }

        /// <summary>
        ///     Some unknown types which are mapped to "label" does not carry a "Description" with them.
        ///     The "Description" is taken as content for "label". If "Description" is not set you run into a System.NullReference
        ///     exception otherwise.
        ///     <see cref="NZazuLabelField.CreateValueControl()" />
        /// </summary>
        private static FieldDefinition ProofForAvailableDescription(FieldDefinition fieldDefinition)
        {
            fieldDefinition.Description = fieldDefinition.Description ?? "-";
            return fieldDefinition;
        }
    }
}