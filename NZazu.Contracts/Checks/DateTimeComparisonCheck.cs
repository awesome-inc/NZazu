using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Castle.Core.Internal;

namespace NZazu.Contracts.Checks
{
    [DisplayName("datetime")]
    public class DateTimeComparisonCheck : IValueCheck
    {
        private readonly FieldDefinition _fieldDefinition;
        private readonly Func<FormData> _formData;
        private readonly int _rowIdx;
        private readonly INZazuTableDataSerializer _tableSerializer;


        public DateTimeComparisonCheck(
            IDictionary<string, string> settings, Func<FormData> formData,
            INZazuTableDataSerializer tableSerializer, int rowIdx,
            FieldDefinition fieldDefinition)

        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            Settings = settings.ToDictionary(x => x.Key, x => (object)x.Value)
                .ToObject<DateTimeComparisonCheckSettings>();

            _rowIdx = rowIdx;
            _fieldDefinition = fieldDefinition ?? throw new ArgumentNullException(nameof(fieldDefinition));
            _tableSerializer = tableSerializer ?? throw new ArgumentNullException(nameof(tableSerializer));

            _formData = formData ?? throw new ArgumentNullException(nameof(formData));
        }

        private DateTimeComparisonCheckSettings Settings { get; }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            string valueToCompareWith;
            if (Settings.TableKey != null && _rowIdx != -1)
                valueToCompareWith = GetValueToCompareWithFromTable();
            else
                _formData().Values.TryGetValue(Settings.FieldToCompareWith, out valueToCompareWith);

            if (value.IsNullOrEmpty() || valueToCompareWith.IsNullOrEmpty())
                return ValueCheckResult.Success; // do not mark not yet set values

            var compareOperation = GetCompareOperation();
            var dateTimeResults = Settings.SpecificDateTimeFormats == null
                ? ParseValuesToDateTime(value, valueToCompareWith)
                : ParseValuesToDateTimeUsingSpecificFormats(value, valueToCompareWith);

            if (!dateTimeResults.ValueAsDateTime.HasValue)
                return new ValueCheckResult(new Exception("Cannot parse source value to datetime."));

            if (!dateTimeResults.ValueToCompareWithAsDateTime.HasValue)
                return new ValueCheckResult(new Exception("Cannot parse 'compareWith' value to datetime."));

            var result = compareOperation(dateTimeResults.ValueAsDateTime.Value,
                dateTimeResults.ValueToCompareWithAsDateTime.Value)
                ? ValueCheckResult.Success
                : new ValueCheckResult(false, new ArgumentException((_fieldDefinition.Prompt ?? _fieldDefinition.Key) + ": " + Settings.Hint));

            return result;
        }

        private string GetValueToCompareWithFromTable()
        {
            // get value to compare by idx
            _formData().Values.TryGetValue(Settings.TableKey, out var tableData);
            var tableDict = _tableSerializer.Deserialize(tableData);

            var tableFieldToCompareWith = $"{Settings.FieldToCompareWith}__{_rowIdx}";
            var tableFieldValueToCompareWithDoesExist =
                tableDict.TryGetValue(tableFieldToCompareWith, out var valueToCompareWith);

            return valueToCompareWith;
        }

        private static DataToCompare ParseValuesToDateTime(string value, string valueToCompareWith)
        {
            DateTime.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var valueDateTime);
            DateTime.TryParse(valueToCompareWith, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var valueToCompareWithDateTime);
            return new DataToCompare(valueDateTime, valueToCompareWithDateTime);
        }

        private DataToCompare ParseValuesToDateTimeUsingSpecificFormats(string value, string valueToCompareWith)
        {
            var canParseValue = DateTime.TryParseExact(
                value,
                Settings.SpecificDateTimeFormatList.ToArray(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var valueDateTime);
            var canParseValueToCompare = DateTime.TryParseExact(
                valueToCompareWith,
                Settings.SpecificDateTimeFormatList.ToArray(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var valueToCompareWithDateTime);
            return new DataToCompare(
                canParseValue ? valueDateTime : (DateTime?)null,
                canParseValueToCompare ? valueToCompareWithDateTime : (DateTime?)null);
        }

        private Compare GetCompareOperation()
        {
            switch (Settings.CompareOperator)
            {
                case "<=": return (value, compare) => value <= compare;
                case "<": return (value, compare) => value < compare;
                case ">=": return (value, compare) => value >= compare;
                case ">": return (value, compare) => value > compare;
                case "=": return (value, compare) => value == compare;
                case "==": return (value, compare) => value == compare;
                default: throw new NotSupportedException("Operator not specified!");
            }
        }

        internal class DateTimeComparisonCheckSettings
        {
            public string CompareOperator { get; set; }
            public string FieldToCompareWith { get; set; }
            public string Hint { get; set; }
            public string TableKey { get; set; }
            public string SpecificDateTimeFormats { get; set; }
            public IEnumerable<string> SpecificDateTimeFormatList => SpecificDateTimeFormats.Split('|');
        }

        private delegate bool Compare(DateTime value, DateTime valueToCompare);

        protected class DataToCompare
        {
            public DataToCompare(DateTime? valueAsDateTime, DateTime? valueToCompareWithAsDateTime)
            {
                ValueAsDateTime = valueAsDateTime;
                ValueToCompareWithAsDateTime = valueToCompareWithAsDateTime;
            }

            public DateTime? ValueAsDateTime { get; }
            public DateTime? ValueToCompareWithAsDateTime { get; }
        }
    }
}