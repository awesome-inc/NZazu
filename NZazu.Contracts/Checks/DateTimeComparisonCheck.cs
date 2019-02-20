using Castle.Core.Internal;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace NZazu.Contracts.Checks
{
    [DisplayName("datetime")]
    public class DateTimeComparisonCheck : IValueCheck
    {
        private delegate bool Compare(DateTime value, DateTime valueToCompare);

        private readonly int _rowIdx;
        private readonly string _compareOperator;
        private readonly string _fieldToCompareWith;
        private readonly string _hint;
        private readonly Func<FormData> _formData;
        private readonly INZazuTableDataSerializer _tableSerializer;
        private readonly string _tableKey;
        private readonly IEnumerable<string> _specificDateTimeFormats;

        public DateTimeComparisonCheck(
            IDictionary<string, string> settings, Func<FormData> formData,
            INZazuTableDataSerializer tableSerializer, int rowIdx)

        //public DateTimeComparisonCheck(
        //    string hint, string compareOperator, string fieldToCompareWith, Func<FormData> formData, INZazuTableDataSerializer tableSerializer, string tableKey = null,
        //    IEnumerable<string> specificDateTimeFormats = null, int rowIdx = -1)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            _rowIdx = rowIdx;
            _tableSerializer = tableSerializer ?? throw new ArgumentNullException(nameof(tableSerializer));

            _hint = settings.Get("Hint", string.Empty);
            _compareOperator = settings.Get("CompareOperator") ?? throw new ArgumentNullException(nameof(settings), "for datetime comparison a 'CompareOperator' must be given");
            _fieldToCompareWith = settings.Get("FieldToCompareWith") ?? throw new ArgumentNullException(nameof(settings), "for datetime comparison a 'FieldToCompareWith' must be given");
            _tableKey = settings.Get("TableToCompareWith");
            _specificDateTimeFormats = settings.Get("SpecificDateTimeFormats", string.Empty).Split('|');

            _formData = formData ?? throw new ArgumentNullException(nameof(formData));
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            string valueToCompareWith;
            if (_tableKey != null && _rowIdx != -1)
            {
                valueToCompareWith = GetValueToCompareWithFromTable();
            }
            else
            {
                _formData().Values.TryGetValue(_fieldToCompareWith, out valueToCompareWith);
            }

            if (value.IsNullOrEmpty() || valueToCompareWith.IsNullOrEmpty()) return ValueCheckResult.Success; // do not mark not yet set values

            var compareOperation = GetCompareOperation();
            var dateTimeResults = _specificDateTimeFormats == null
                ? ParseValuesToDateTime(value, valueToCompareWith)
                : ParseValuesToDateTimeUsingSpecificFormats(value, valueToCompareWith);

            if (!dateTimeResults.ValueAsDateTime.HasValue || !dateTimeResults.ValueToCompareWithAsDateTime.HasValue)
                return new ValueCheckResult(new Exception("Cannot parse value to datetime."));

            var result = compareOperation(dateTimeResults.ValueAsDateTime.Value, dateTimeResults.ValueToCompareWithAsDateTime.Value)
                ? ValueCheckResult.Success
                : new ValueCheckResult(false, new ArgumentException(_hint));

            return result;
        }

        private string GetValueToCompareWithFromTable()
        {
            // get value to compare by idx
            _formData().Values.TryGetValue(_tableKey, out var tableData);
            var tableDict = _tableSerializer.Deserialize(tableData);

            var tableFieldToCompareWith = $"{_fieldToCompareWith}__{_rowIdx}";
            var tableFieldValueToCompareWithDoesExist =
                tableDict.TryGetValue(tableFieldToCompareWith, out var valueToCompareWith);

            return valueToCompareWith;
        }


        protected class DataToCompare
        {
            public DateTime? ValueAsDateTime { get; }
            public DateTime? ValueToCompareWithAsDateTime { get; }

            public DataToCompare(DateTime? valueAsDateTime, DateTime? valueToCompareWithAsDateTime)
            {
                ValueAsDateTime = valueAsDateTime;
                ValueToCompareWithAsDateTime = valueToCompareWithAsDateTime;
            }
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
                _specificDateTimeFormats.ToArray(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var valueDateTime);
            var canParseValueToCompare = DateTime.TryParseExact(
                valueToCompareWith,
                _specificDateTimeFormats.ToArray(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var valueToCompareWithDateTime);
            return new DataToCompare(
                canParseValue ? valueDateTime : (DateTime?)null,
                canParseValueToCompare ? valueToCompareWithDateTime : (DateTime?)null);
        }

        private Compare GetCompareOperation()
        {
            switch (_compareOperator)
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

        public static IValueCheck Create(IDictionary<string, string> settings, Func<FormData> formData, INZazuTableDataSerializer tableSerializer, int rowIdx)
        {
            Assert.Fail("implement me");
            return null;
            //if (values == null || values.Count < 3) throw new ArgumentException("Hint, comparison operator and field id to compare values needs to be specified");
            //if (values[1] != "<=" && values[1] != ">=" && values[1] != "=" && values[1] != "<" && values[1] != ">") throw new ArgumentException("Only <=, >=, =, < and > are supported operators!");
            //var optionalDateFormat = values.Count >= 4 ? values[3].Split('|') : null;
            //var tableId = values.Count >= 5 && values[4] != string.Empty ? values[4] : null;

            //return new DateTimeComparisonCheck(values[0], values[1], values[2], formData, tableSerializer, tableId, optionalDateFormat, rowIdx);
        }
    }
}